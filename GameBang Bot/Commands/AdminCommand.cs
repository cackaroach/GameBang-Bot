using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GameBang_Bot.CommandBases;
using GameBang_Bot.Database.Models;
using GameBang_Bot.Discord;
using GameBang_Bot.Properties;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameBang_Bot.Commands {
	[Channel]
	[Name("MatchAdminChannel")]
	[Summary("서비스 운용에 필요한 명령어를 사용하는 채널이에요.")]
	public class AdminCommand : CommandBase {
		private async Task<LolTeam> GetOrCreateTeamAsync(string team) {
			try {
				return DbContext.LolTeams.Single(p => p.Name == team);
			} catch (InvalidOperationException) {
				await DbContext.LolTeams.AddAsync(new LolTeam() {
					Name = team
				});
				await DbContext.SaveChangesAsync();
				return DbContext.LolTeams.Single(p => p.Name == team);
			}
		}

		[Command("등록")]
		[Summary("'A vs B' 경기를 등록합니다.")]
		public async Task Create([Summary("A팀")] string t1, [Summary("B팀")] string t2) {
			t1 = t1.ToUpper();
			t2 = t2.ToUpper();

			LolTeam team1 = await GetOrCreateTeamAsync(t1), team2 = await GetOrCreateTeamAsync(t2);

			if (DbContext.Matches.AsQueryable().Where(p => (p.Team1 == team1.Id || p.Team2 == team1.Id || p.Team1 == team2.Id || p.Team2 == team2.Id) && p.Win == null).Count() > 0) {
				await ReplyAsync("이미 팀 이름으로 등록되어 있는 게임이 있어요");
				return;
			}

			var matchChannel = Context.Guild.TextChannels.Single(p => p.Id == PropertiesContext.Channel.MatchChannelId);
			var message = await matchChannel.SendMessageAsync("새로운 경기가 등록되었어요!");

			await DbContext.Matches.AddAsync(new Match() {
				Id = message.Id,
				Team1 = team1.Id,
				Team2 = team2.Id
			});
			await DbContext.SaveChangesAsync();

			var embed = new EmbedBuilder() {
				Title = $"[{t1} vs {t2}]",
				Description = $"[{t1} vs {t2}] 경기의 승리 팀을 예측해보세요.",
				Timestamp = DateTime.Now
			};

			try {
				var stream = ImageRender.GetMatchImageStream(t1, t2);
				var msg = await Context.Channel.SendFileAsync(stream, "background.png", string.Empty);
				embed.ImageUrl = msg.Attachments.First().Url;
			} catch { }

			embed.ThumbnailUrl = PropertiesContext.Images.BetThumbnail;
			embed.AddField("참가 팀", $"{t1} vs {t2}");
			embed.AddField($"{t1} 포인트", "0 (0%)", true);
			embed.AddField($"{t2} 포인트", "0 (0%)", true);
			embed.Color = Color.Green;

			await message.ModifyAsync(p => {
				p.Content = null;
				p.Embed = embed.Build();
			});
		}

		[Command("시작")]
		[Summary("경기를 시작하고 베팅할 수 없게 합니다.")]
		public async Task Begin([Summary("경기코드")] ulong team) {
			var match = DbContext.Matches.Single(p => p.Id == team && p.IsBetable == true);
			var message = await Context.Guild.TextChannels.Single(p => p.Id == PropertiesContext.Channel.MatchChannelId).GetMessageAsync(match.Id) as IUserMessage;

			match.IsBetable = false;
			await DbContext.SaveChangesAsync();

			var embed = message.Embeds.First().ToEmbedBuilder();
			embed.Color = Color.Orange;
			embed.Description = $"{embed.Description.Split("]")[0]}] 경기가 진행 중입니다!\n※ 진행 중인 게임의 승부는 예측하실 수 없습니다.";
			await message.ModifyAsync(p => p.Embed = embed.Build());
		}

		[Command("재개")]
		[Summary("진행 중인 경기를 재개하고 베팅할 수 있게 합니다.")]
		public async Task Resume([Summary("경기코드")] ulong team) {
			var match = DbContext.Matches.Single(p => p.Id == team && p.IsBetable == false);
			var message = await Context.Guild.TextChannels.Single(p => p.Id == PropertiesContext.Channel.MatchChannelId).GetMessageAsync(match.Id) as IUserMessage;

			match.IsBetable = true;
			await DbContext.SaveChangesAsync();

			var embed = message.Embeds.First().ToEmbedBuilder();
			embed.Color = Color.Green;
			embed.Description = $"{embed.Description.Split("]")[0]}] 경기의 승리 팀을 예측해보세요.";
			await message.ModifyAsync(p => p.Embed = embed.Build());
		}

		[Command("결과")]
		[Summary("경기의 우승 팀을 설정하고, 우승 포인트를 지급합니다.")]
		public async Task SetWinner([Summary("우승팀")] string teamName) {
			teamName = teamName.ToUpper();
			var winner = await DbContext.LolTeams.SingleAsync(p => p.Name == teamName);
			var match = DbContext.Matches.AsQueryable().Single(p => p.Win == null && (p.Team1 == winner.Id || p.Team2 == winner.Id));
			var channel = Context.Guild.TextChannels.Single(p => p.Id == PropertiesContext.Channel.OverMatchChannelId);
			var message = await Context.Guild.TextChannels.Single(p => p.Id == PropertiesContext.Channel.MatchChannelId).GetMessageAsync(match.Id);

			var usersWin = DbContext.UserBets.AsQueryable().Where(p => p.MatchId == match.Id && p.Earned == null && p.TeamId == winner.Id);
			var usersLost = DbContext.UserBets.AsQueryable().Where(p => p.MatchId == match.Id && p.Earned == null && p.TeamId != winner.Id);
			var team1Points = usersWin.Sum(s => s.Point);
			var team2Points = usersLost.Sum(s => s.Point);
			var team1Name = DbContext.LolTeams.Single(p => p.Id == match.Team1).Name;
			var team2Name = DbContext.LolTeams.Single(p => p.Id == match.Team2).Name;

			foreach (var u in usersWin)
				u.Earned = (int)(team2Points * (u.Point / (double)team1Points) + ((double)u.Point * 0.95));

			foreach (var u in usersLost)
				u.Earned = 0;

			match.IsBetable = false;
			match.Win = winner.Id;

			var embed = new EmbedBuilder() {
				Title = $"{teamName} 승리",
				Description = $"[{team1Name} vs {team2Name}] 경기에서 {winner.Name} 팀이 우승하였습니다!",
				Timestamp = DateTime.Now
			};

			var rankers = usersWin.OrderByDescending(p => p.Point).Take(10);
			var sb = new StringBuilder();
			foreach (var r in rankers)
				sb.AppendLine($"{Context.Guild.GetUser(r.UserId)?.Mention}: {r.Earned} 포인트");

			embed.AddField("경기", $"{team1Name} vs {team2Name}", true);
			embed.AddField("우승 팀", teamName, true);
			if (sb.Length > 0)
				embed.AddField("우승자 랭킹", sb.ToString());
			embed.ThumbnailUrl = PropertiesContext.Images.BetThumbnail;
			embed.Color = Color.Blue;

			var oembed = message.Embeds.First();
			if (oembed.Image.Value.Url != null) {
				using (var wc = new WebClient()) {
					var bytes = wc.DownloadData(oembed.Image.Value.Url);
					var ms = new MemoryStream(bytes);

					try {
						ms = ImageRender.GetOverImageStream(ms, teamName == team1Name);
						var adminChannel = Context.Guild.TextChannels.Single(p => p.Id == PropertiesContext.Channel.MatchAdminChannelId);

						var msg = await adminChannel.SendFileAsync(ms, "background.png", string.Empty);
						embed.ImageUrl = msg.Attachments.First().Url;
					} catch { }
				}
			}

			await DbContext.SaveChangesAsync();
			await channel.SendMessageAsync(embed: embed.Build());
			await message.DeleteAsync();
		}
	}
}
