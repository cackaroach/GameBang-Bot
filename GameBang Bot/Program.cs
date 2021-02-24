using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GameBang_Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GameBang_Bot {
	class Program {
		public static void Main(string[] args) {
			var commands = new CommandService(
				new CommandServiceConfig {
					DefaultRunMode = RunMode.Async,
					LogLevel = LogSeverity.Verbose
				}
			);

			var client = new DiscordSocketClient(
				new DiscordSocketConfig {
					LogLevel = LogSeverity.Verbose
				}
			);

			var services = new ServiceCollection()
				 .AddSingleton(client)
				 .AddSingleton(commands)
				 .AddSingleton<CommandHandler>()
				 .AddSingleton<LoggingService>()
				 .AddSingleton<StartUpService>()
				 .AddSingleton<ReportService>();

			var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

			StartServices(provider).GetAwaiter().GetResult();
		}

		private static async Task StartServices(IServiceProvider provider) {
			provider.GetRequiredService<LoggingService>();
			await provider.GetRequiredService<StartUpService>().StartAsync();
			provider.GetRequiredService<CommandHandler>();
			provider.GetRequiredService<ReportService>();

			await Task.Delay(-1);
		}
	}
}
