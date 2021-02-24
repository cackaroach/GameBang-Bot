using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GameBang_Bot.Properties;
using System.Reflection;
using System.Threading.Tasks;

namespace GameBang_Bot.Services {
	class StartUpService {
		private readonly DiscordSocketClient discord;
		private readonly CommandService commands;

		public StartUpService(DiscordSocketClient discord, CommandService commands) {
			this.discord = discord;
			this.commands = commands;
		}

		public async Task StartAsync() {
			await discord.LoginAsync(TokenType.Bot, PropertiesContext.Discord.Token);
			await discord.StartAsync();

			await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
		}
	}
}
