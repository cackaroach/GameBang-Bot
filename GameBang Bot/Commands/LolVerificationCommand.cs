using Discord;
using Discord.Commands;
using GameBang_Bot.CommandBases;
using GameBang_Bot.Database.Models;
using GameBang_Bot.Discord;
using GameBang_Bot.LeagueOfLegends.Converters;
using GameBang_Bot.LeagueOfLegends.Enums;
using GameBang_Bot.Properties;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameBang_Bot.Commands {
	[Channel]
	[Name("LolVerificationChannel")]
	[Summary("서버에서 지원하는 다양한 이벤트에 참여하기 위해 롤 계정을 인증하는 채널이에요.")]
	public class LolVerificationCommand : CommandBase {
		[Command("인증")]
		[Summary("닉네임으로 롤 계정을 인증합니다.")]
		public async Task Create([Summary("닉네임")] string username) {
			using (var parser = new LeagueOfLegends.LolParser()) {
				try {
					var summoner = await parser.ParseSummonerAsync(username);
					var leagueList = await parser.ParseLeagueEntryListAsync(summoner.Id);
					var user = DbContext.Users.AsNoTracking().SingleOrDefault((p) => p.Id == Context.User.Id);

					if (user == null) {
						await DbContext.Database.ExecuteSqlRawAsync($"INSERT INTO users VALUES({Context.User.Id})");
						user = DbContext.Users.AsNoTracking().Single((p) => p.Id == Context.User.Id);
					}

					var highTier = TierEnum.Unranked;
					var rank = string.Empty;
					var discordUser = Context.User as IGuildUser;

					var embed = GeneralEmbedBuilder.GeneralEmbed(Context, "인증 성공", $"{Context.User.Mention}님의 티어가 정상적으로 인증되었습니다.");
					embed.AddField("소환사 이름", username);

					var roleIds = discordUser.RoleIds;
					foreach (var r in roleIds)
						if (PropertiesContext.Role.IsRankRole(r))
							await discordUser.RemoveRoleAsync(Context.Guild.GetRole(r));

					foreach (var l in leagueList) {
						switch (l.QueueType) {
							case QueueTypeEnum.RankedSolo:
								embed.AddField("개인 랭크", $"{l.Tier} {l.Rank}");
								await discordUser.AddRoleAsync(Context.Guild.GetRole(PropertiesContext.Role.ToId("Sole" + l.Tier.ToString())));

								if (highTier < l.Tier) {
									highTier = l.Tier;
									rank = l.Rank;
								}
								break;
							case QueueTypeEnum.RankedTeam:
								embed.AddField("자유 랭크", $"{l.Tier} {l.Rank}");
								await discordUser.AddRoleAsync(Context.Guild.GetRole(PropertiesContext.Role.ToId("Team" + l.Tier.ToString())));

								if (highTier < l.Tier) {
									highTier = l.Tier;
									rank = l.Rank;
								}
								break;
						}
					}

					await DbContext.SaveChangesAsync();
					embed.ThumbnailUrl = TierConverter.GetTierUri(highTier, rank);
					embed.Color = TierConverter.GetTierColor(highTier);

					await ReplyAsync(embed: embed.Build());
				} catch (Exception e) {
					var embed = GeneralEmbedBuilder.ErrorEmbed(Context, e.Message);
					embed.Fields.Clear();
					embed.Title = "인증 실패";
					embed.Description = "티어 인증에 실패했습니다.";
					embed.Color = Color.Red;
					embed.AddField("소환사 이름", username,true);
					embed.AddField("오류 사유", e.Message, true);
					await ReplyAsync(embed: embed.Build());
				}
			}
		}
	}
}
