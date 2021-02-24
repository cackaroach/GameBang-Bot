using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GameBang_Bot.Services {
	class LoggingService {
		private const string LOG_DIRECTORY = "log";

		private readonly DiscordSocketClient discord = null;
		private readonly CommandService commands = null;
		private static readonly StreamWriter streamWriter = new StreamWriter($"{LOG_DIRECTORY}/{DateTime.Now.ToString("yy-MM-dd HH-mm-ss")}.log");

		public LoggingService(DiscordSocketClient discord, CommandService commands) {
			this.discord = discord;
			this.commands = commands;

			this.discord.Log += OnLogAsync;
			this.commands.Log += OnLogAsync;

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			if (!Directory.Exists(LOG_DIRECTORY))
				Directory.CreateDirectory(LOG_DIRECTORY);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			var ex = e.ExceptionObject as Exception;
			streamWriter.WriteLine(ex.Message);
			streamWriter.WriteLine(ex.StackTrace);
			streamWriter.WriteLine(ex.Source);
			streamWriter.Flush();
		}

		private Task OnLogAsync(LogMessage msg) {
			string log = $"{DateTime.Now.ToString("yy/MM/dd HH:mm:ss")} [{msg.Severity}] \t{msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
			streamWriter.WriteLine(log);
			streamWriter.Flush();

			return Console.Out.WriteLineAsync(log);
		}


	}
}
