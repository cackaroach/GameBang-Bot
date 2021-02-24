using Discord;
using Discord.Commands;
using GameBang_Bot.CommandBases;
using GameBang_Bot.Database.Models;
using GameBang_Bot.Discord;
using GameBang_Bot.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameBang_Bot.Commands {
	[Channel]
	[Name("BettingChannel")]
	[Summary("승부 예측 서비스를 진행할 수 있는 채널이에요")]
	public class BettingCommand : CommandBase {
		[Command("베팅")]
		[Summary("진행 중인 게임에 승부 예측합니다.")]
		public async Task Bet([Summary("팀이름")] string teamName, [Summary("포인트")] int point) {
			teamName = teamName.ToUpper();
			var user = await DbContext.Users.AsNoTracking().SingleOrDefaultAsync(p => p.Id == Context.User.Id);

			if (user == null)
				throw new Exception($"인증되지 않은 사용자입니다.");

			var team = await DbContext.LolTeams.AsQueryable().SingleOrDefaultAsync(p => p.Name == teamName);

			if (team == null)
				throw new Exception($"{teamName}팀을 찾을 수 없습니다.");

			var match = await DbContext.Matches.AsQueryable().SingleOrDefaultAsync(p => (p.Team1 == team.Id || p.Team2 == team.Id) && p.IsBetable == true);

			if (match == null)
				throw new Exception($"{teamName}팀이 진행 중인 경기를 찾을 수 없습니다.");

			if ((int)user.Point >= point) {
				UserBet bet = DbContext.UserBets.FirstOrDefault(p => p.UserId == Context.User.Id && p.MatchId == match.Id && p.TeamId == team.Id);
				if (bet == null) {
					bet = new UserBet() {
						UserId = Context.User.Id,
						MatchId = match.Id,
						TeamId = team.Id,
						Point = point
					};
					await DbContext.UserBets.AddAsync(bet);
				} else
					bet.Point += point;

				await DbContext.SaveChangesAsync();
				var embed = GeneralEmbedBuilder.GeneralEmbed(Context, "승부 예측", $"{Context.User.Mention}님이 {teamName} 팀의 승리를 예측하셨어요!");
				embed.AddField("경기", $"{DbContext.LolTeams.Single(p => p.Id == match.Team1).Name} vs {DbContext.LolTeams.Single(p => p.Id == match.Team2).Name}");
				embed.AddField("예측 포인트", bet.Point, true);
				embed.AddField("잔여 포인트", (int)user.Point - point, true);
				await ReplyAsync(embed: embed.Build());

				var message = await Context.Guild.TextChannels.First(p => p.Id == PropertiesContext.Channel.MatchChannelId).GetMessageAsync(match.Id) as IUserMessage;
				var team1Points = DbContext.UserBets.AsQueryable().Where(p => p.MatchId == match.Id && p.Earned == null && p.TeamId == match.Team1).Sum(s => s.Point);
				var team2Points = DbContext.UserBets.AsQueryable().Where(p => p.MatchId == match.Id && p.Earned == null && p.TeamId == match.Team2).Sum(s => s.Point);
				var oembed = message.Embeds.First().ToEmbedBuilder();
				oembed.Fields.RemoveRange(1, 2);
				oembed.AddField($"{DbContext.LolTeams.Single(p => p.Id == match.Team1).Name} 포인트", $"{team1Points} ({Math.Round(((double)team1Points / (team1Points + team2Points) * 100), 1)}%)", true);
				oembed.AddField($"{DbContext.LolTeams.Single(p => p.Id == match.Team2).Name} 포인트", $"{team2Points} ({Math.Round(((double)team2Points / (team1Points + team2Points) * 100), 1)}%)", true);
				await message.ModifyAsync(p => p.Embed = oembed.Build());
			} else
				throw new Exception($"보유한 포인트({user.Point})가 부족합니다.");
		}


		[Command("포인트"), Alias("내포인트")]
		[Summary("현재 가지고 있는 포인트를 확인합니다.")]
		public async Task MyPoint() {
			var user = await DbContext.Users.AsNoTracking().SingleOrDefaultAsync(p => p.Id == Context.User.Id);
			if(user == null)
				throw new Exception($"인증되지 않은 사용자입니다.");

			var embed = GeneralEmbedBuilder.GeneralEmbed(Context, "내 포인트", $"{Context.User.Mention}님이 현재 포인트는 {user.Point} 포인트입니다.");
			embed.AddField("잔여 포인트", user.Point);
			await ReplyAsync(embed: embed.Build());
		}

		[Command("내예측"), Alias("예측경기")]
		[Summary("진행 중인 경기 중 예측한 경기 리스트를 살펴봅니다.")]
		public async Task MyBet() {
			var bets = DbContext.UserBets.AsEnumerable().Where(p => p.UserId == Context.User.Id && p.Earned == null).ToArray();
			var embed = GeneralEmbedBuilder.GeneralEmbed(Context, "예측한 경기", $"진행 중인 경기들 중 {Context.User.Mention}님께서 예측하신 경기 리스트입니다.");

			foreach (var b in bets) {
				var match = DbContext.Matches.SingleOrDefault(p => p.Win == null && (p.Team1 == b.TeamId || p.Team2 == b.TeamId));
				embed.AddField($"{DbContext.LolTeams.Single(p => p.Id == match.Team1).Name} vs {DbContext.LolTeams.Single(p => p.Id == match.Team2).Name}", $"{DbContext.LolTeams.Single(p => p.Id == b.TeamId).Name}: {b.Point} 포인트");
			}
			await ReplyAsync(embed: embed.Build());
		}
	}
}
