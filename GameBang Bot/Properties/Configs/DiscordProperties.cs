namespace GameBang_Bot.Properties.Configs {
	class DiscordProperties : IProperties {
		public string Token { get; set; }
		public string Prefix { get; set; }
		public ulong ServerId { get; set; }
	}
}
