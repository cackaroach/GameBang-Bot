using Discord;
using Discord.Commands;
using GameBang_Bot.CommandBases;
using GameBang_Bot.Database.Models;
using GameBang_Bot.Discord;
using GameBang_Bot.LeagueOfLegends.Enums;
using GameBang_Bot.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameBang_Bot.Commands {
	[Channel]
	[Name("AttendanceChannel")]
	[Summary("출석 체크하는 채널이에요.")]
	public class AttendanceCommand : CommandBase {
		[Command("출석체크"), Alias("출첵", "출석", "출쳌", "쳌", "첵", "체크", "출", "ㅊ", "ㅊㅅ", "ㅊㅊ", "c", "cc", "ct", "추럭", "cnftjr", "cnfcpr", "cnfcpz", "cnf", "출서", "춠")]
		[Summary("출석 체크합니다.")]
		public async Task Begin() {
			var user = await DbContext.Users.AsNoTracking().SingleOrDefaultAsync(p => p.Id == Context.User.Id);

			if (user == null)
				throw new Exception("인증되지 않은 사용자입니다.");

			var last = DbContext.UserPoints.AsQueryable().OrderByDescending(p => p.Date).FirstOrDefault(p => p.Reason == "Attendance");

			if (last == null || last?.Date.AddMinutes(PropertiesContext.Attendance.Span) < DateTime.Now || last.Date.Day != DateTime.Now.Day) {
				DbContext.UserPoints.Add(new UserPoint() {
					UserId = user.Id,
					Reason = "Attendance",
					Point = PropertiesContext.Attendance.Point
				});

				await DbContext.SaveChangesAsync();

				var embed = GeneralEmbedBuilder.SuccessEmbed(Context, "출석 체크 성공", $"{Context.User.Mention}님이 출석 체크하셨습니다.\n{PropertiesContext.Attendance.Span}분 간격으로 출석 체크해서 {PropertiesContext.Attendance.Point} 포인트를 얻으실 수 있어요.");
				embed.Color = Color.Green;
				embed.AddField("포인트", (int)user.Point + PropertiesContext.Attendance.Point, true);
				embed.AddField("다음 출석", DateTime.Now.AddMinutes(PropertiesContext.Attendance.Span).ToString("tt h시 m분"), true);

				await ReplyAsync(embed: embed.Build());
			} else
				throw new Exception($"출석 체크할 수 없습니다.\n{last.Date.AddMinutes(PropertiesContext.Attendance.Span).ToString("tt h시 m분")}에 다시 시도해주세요.");
		}
	}
}