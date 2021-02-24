using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GameBang_Bot.CommandBases;
using GameBang_Bot.Discord;
using GameBang_Bot.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBang_Bot.Services {
	class CommandHandler {
		private readonly DiscordSocketClient discord;
		private readonly CommandService commands;
		private readonly IServiceProvider provider;

		public CommandHandler(DiscordSocketClient discord, CommandService commands, IServiceProvider provider) {
			this.discord = discord;
			this.commands = commands;
			this.provider = provider;

			this.discord.MessageReceived += OnMessageReceivedAsync;
			this.commands.CommandExecuted += Commands_CommandExecuted;
		}

		private async Task Commands_CommandExecuted(Optional<CommandInfo> arg1, ICommandContext arg2, IResult arg3) {
			switch(arg3.Error) {
				case CommandError.Exception:
					if(arg3 is ExecuteResult ex) {
						if(ex.Exception.InnerException != null)
							await arg2.Channel.SendMessageAsync(embed: GeneralEmbedBuilder.ErrorEmbed(arg2, $"{ex.Exception.InnerException.Message}").Build());
						else 
							await arg2.Channel.SendMessageAsync(embed: GeneralEmbedBuilder.ErrorEmbed(arg2, $"{ex.Exception.Message}").Build());
					} else
						await arg2.Channel.SendMessageAsync(embed: GeneralEmbedBuilder.ErrorEmbed(arg2, arg3.ErrorReason).Build());
					break;
				case CommandError.UnknownCommand:
					await arg2.Channel.SendMessageAsync(embed: GeneralEmbedBuilder.UnknownCommandEmbed(arg2, arg2.Message));
					break;
			}
		}

		private async Task OnMessageReceivedAsync(SocketMessage socketMessage) {
			SocketUserMessage message = socketMessage as SocketUserMessage;

			if (message == null || message.Author == discord.CurrentUser)
				return;

			int argPos = 0;
			SocketCommandContext context = new SocketCommandContext(discord, message);

			if (message.HasStringPrefix(PropertiesContext.Discord.Prefix, ref argPos) || message.HasMentionPrefix(discord.CurrentUser, ref argPos)) {
				switch (message.Content[1..].ToLower()) {
					case "help":
					case "?":
					case "도움말":
					case "명령어":
						var embed = GeneralEmbedBuilder.HelpEmbed(context, commands);
						if(embed != null)
							await context.Channel.SendMessageAsync(embed: embed);
						break;
					default:
						IResult result = await commands.ExecuteAsync(context, argPos, provider);
						break;
				}

			}
		}
	}
}
