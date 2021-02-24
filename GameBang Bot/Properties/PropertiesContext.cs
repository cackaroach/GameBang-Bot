using GameBang_Bot.Properties.Configs;

namespace GameBang_Bot.Properties {
	class PropertiesContext {
		private const string PROPERTIES_DIRECTORY = "properties";

		private static ChannelProperties channel = null;
		public static ChannelProperties Channel {
			get => (channel ??= PropertiesReader.ReadAsObject<ChannelProperties>($"{PROPERTIES_DIRECTORY}/channel.properties"));
		}

		private static RoleProperties role = null;
		public static RoleProperties Role {
			get => (role ??= PropertiesReader.ReadAsObject<RoleProperties>($"{PROPERTIES_DIRECTORY}/role.properties"));
		}

		private static DatabaseProperties database = null;
		public static DatabaseProperties Database {
			get => (database ??= PropertiesReader.ReadAsObject<DatabaseProperties>($"{PROPERTIES_DIRECTORY}/database.properties"));
		}

		private static DiscordProperties discord = null;
		public static DiscordProperties Discord {
			get => (discord ??= PropertiesReader.ReadAsObject<DiscordProperties>($"{PROPERTIES_DIRECTORY}/discord.properties"));
		}

		private static LeagueOfLegendsProperties leagueOfLegends = null;
		public static LeagueOfLegendsProperties LeagueOfLegends {
			get => (leagueOfLegends ??= PropertiesReader.ReadAsObject<LeagueOfLegendsProperties>($"{PROPERTIES_DIRECTORY}/leagueoflegends.properties"));
		}

		private static AttendanceProperties attendance = null;
		public static AttendanceProperties Attendance {
			get => (attendance ??= PropertiesReader.ReadAsObject<AttendanceProperties>($"{PROPERTIES_DIRECTORY}/attendance.properties"));
		}

		private static ImagesProperties images = null;
		public static ImagesProperties Images {
			get => (images ??= PropertiesReader.ReadAsObject<ImagesProperties>($"{PROPERTIES_DIRECTORY}/images.properties"));
		}
	}
}
