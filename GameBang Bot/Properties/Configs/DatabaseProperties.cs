namespace GameBang_Bot.Properties.Configs {
	class DatabaseProperties : IProperties {
		public string Host { get; set; }
		public string Database { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int Port { get; set; }
	}
}
