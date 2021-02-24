using GameBang_Bot.LeagueOfLegends.Enums;
using Newtonsoft.Json;
using System;

namespace GameBang_Bot.LeagueOfLegends.Converters {
	class QueueTypeConverter : JsonConverter {
		public override bool CanConvert(Type objectType) {
			return typeof(Enums.TierEnum).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			switch ((reader.Value as string).ToLower()) {
				case "ranked_solo_5x5":
					return QueueTypeEnum.RankedSolo;
				case "ranked_team_5x5":
					return QueueTypeEnum.RankedTeam;
				default:
					return QueueTypeEnum.Unknown;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

		}
	}
}
