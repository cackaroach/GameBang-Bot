namespace GameBang_Bot.Properties.Configs {
	class ChannelProperties : IProperties {
		public ulong MatchAdminChannelId { get; set; }
		public ulong UserAdminChannelId { get; set; }
		public ulong AttendanceChannelId { get; set; }
		public ulong LolVerificationChannelId { get; set; }
		public ulong MatchChannelId { get; set; }
		public ulong OverMatchChannelId { get; set; }
		public ulong BettingChannelId { get; set; }
		public ulong ReportChannelId { get; set; }
		public ulong ReportAdminChannelId { get; set; }
	}
}
