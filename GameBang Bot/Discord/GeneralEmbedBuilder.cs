using Discord;
using Discord.Commands;
using GameBang_Bot.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBang_Bot.Discord {
	static class GeneralEmbedBuilder {
		public static EmbedBuilder GeneralEmbed(ICommandContext context) {
			return GeneralEmbed(context, string.Empty, string.Empty);
		}

		public static EmbedBuilder GeneralEmbed(ICommandContext context, string title) {
			return GeneralEmbed(context, title, string.Empty);
		}

		public static EmbedBuilder GeneralEmbed(ICommandContext context, string title, string description) {
			var embed = new EmbedBuilder() {
				Timestamp = DateTime.Now,
				Title = title,
				Description = description,
				Footer = new EmbedFooterBuilder() {
					IconUrl = context.User.GetAvatarUrl() ?? context.User.GetDefaultAvatarUrl(),
					Text = context.User.Username
				},
				Color = Color.Green
			};

			return embed;
		}

		public static EmbedBuilder SuccessEmbed(ICommandContext context, string title, string desc) {
			var embed = GeneralEmbed(context, title, desc);
			embed.ThumbnailUrl = PropertiesContext.Images.Success;
			return embed;
		}

		public static EmbedBuilder ErrorEmbed(ICommandContext context, string message) {
			var embed = new EmbedBuilder() {
				Title = "예외 발생",
				Description = "명령어 실행 중 오류가 발생하였습니다.",
				Timestamp = DateTime.Now,
				Color = Color.Red,
				ThumbnailUrl = PropertiesContext.Images.Exception,
				Footer = new EmbedFooterBuilder() {
					IconUrl = context.User.GetAvatarUrl() ?? context.User.GetDefaultAvatarUrl(),
					Text = context.User.Username
				}
			};

			embed.AddField("오류 내용", message);

			return embed;
		}

		public static Embed UnknownCommandEmbed(ICommandContext context, IUserMessage socketMessage) {
			var embed = new EmbedBuilder() {
				Color = Color.Red,
				Title = "명령어 오류",
				Description = $"{context.User.Mention}님께서 입력하신 명령어는 없는 명령어입니다.\n명령어 리스트를 보시려면 !명령어, !help, !? 등을 입력하세요.",
				ThumbnailUrl = PropertiesContext.Images.Unknown,
				Timestamp = DateTime.Now,
				Fields = new List<EmbedFieldBuilder>() {
									new EmbedFieldBuilder() {
										Name = "입력된 명령어",
										Value = socketMessage.Content
									}
								},
				Footer = new EmbedFooterBuilder() {
					IconUrl = context.User.GetAvatarUrl() ?? context.User.GetDefaultAvatarUrl(),
					Text = context.User.Username
				}
			};

			return embed.Build();
		}

		public static Embed HelpEmbed(ICommandContext context, CommandService commands) {
			var embed = new EmbedBuilder() {
				Color = Color.Orange,
				Title = "명령어",
				ThumbnailUrl = PropertiesContext.Images.Help,
				Timestamp = DateTime.Now,
				Footer = new EmbedFooterBuilder() {
					IconUrl = context.User.GetAvatarUrl() ?? context.User.GetDefaultAvatarUrl(),
					Text = context.User.Username
				}
			};

			foreach (var m in commands.Modules) {
				if (m.Preconditions.Count > 0) {
					var type = PropertiesContext.Channel.GetType();
					var property = type.GetProperty(m.Name + "Id");
					ulong channelId = (UInt64)property.GetValue(PropertiesContext.Channel);

					if (channelId == context.Channel.Id) {
						embed.Description = m.Summary;

						foreach (var c in m.Commands) {
							var param = new StringBuilder($"!{c.Name}");
							foreach (var p in c.Parameters)
								if (p.Summary != null)
									param.Append($" {p.Summary}");
								else
									param.Append($" {p.Name}");

							StringBuilder alias = null;
							if (c.Aliases.Count > 1) {
								alias = new StringBuilder($"동명령어:");
								foreach (var a in c.Aliases)
									alias.Append($" !{a}");
							}

							var sum = c.Summary;
							if (alias != null)
								sum += "\n" + alias.ToString();
							embed.AddField(param.ToString(), sum);
						}
					}
				}
			}

			if (embed.Fields.Count > 0)
				return embed.Build();
			else
				return null;
		}
	}
}
