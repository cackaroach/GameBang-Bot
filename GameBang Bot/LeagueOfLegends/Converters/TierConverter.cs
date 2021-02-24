using Discord;
using GameBang_Bot.LeagueOfLegends.Enums;
using Newtonsoft.Json;
using System;

namespace GameBang_Bot.LeagueOfLegends.Converters {
	class TierConverter : JsonConverter {
		public static string GetTierUri(TierEnum tier, string rankRome = "I") {
			var rank = rankRome.Length;
			rank = (rankRome == "IV") ? 4 : rank;

			switch (tier) {
				case TierEnum.Unranked:
					return "https://opgg-static.akamaized.net/images/medals/default.png";
				case TierEnum.Iron:
					return $"https://opgg-static.akamaized.net/images/medals/iron_{rank}.png";
				case TierEnum.Bronze:
					return $"https://opgg-static.akamaized.net/images/medals/bronze_{rank}.png";
				case TierEnum.Silver:
					return $"https://opgg-static.akamaized.net/images/medals/silver_{rank}.png";
				case TierEnum.Gold:
					return $"https://opgg-static.akamaized.net/images/medals/gold_{rank}.png";
				case TierEnum.Platinum:
					return $"https://opgg-static.akamaized.net/images/medals/platinum_{rank}.png";
				case TierEnum.Diamond:
					return $"https://opgg-static.akamaized.net/images/medals/diamond_{rank}.png";
				case TierEnum.Master:
					return $"https://opgg-static.akamaized.net/images/medals/master_{rank}.png";
				case TierEnum.GrandMaster:
					return $"https://opgg-static.akamaized.net/images/medals/grandmaster_{rank}.png";
				case TierEnum.Challenger:
					return $"https://opgg-static.akamaized.net/images/medals/challenger_{rank}.png";
				default:
					return string.Empty;
			}
		}

		public static Color GetTierColor(TierEnum tier) {
			switch (tier) {
				case TierEnum.Iron:
					return new Color(56, 47, 45);
				case TierEnum.Bronze:
					return new Color(160, 96, 60);
				case TierEnum.Silver:
					return new Color(135, 158, 166);
				case TierEnum.Gold:
					return new Color(255, 215, 0);
				case TierEnum.Platinum:
					return new Color(101, 227, 163);
				case TierEnum.Diamond:
					return new Color(130, 73, 213);
				case TierEnum.Master:
					return new Color(209, 31, 222);
				case TierEnum.GrandMaster:
					return new Color(228, 32, 37);
				case TierEnum.Challenger:
					return new Color(255, 253, 229);
				default:
					return new Color(70, 70, 70);
			}
		}

		public override bool CanConvert(Type objectType) {
			return typeof(Enums.TierEnum).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			switch ((reader.Value as string).ToLower()) {
				case "iron":
					return TierEnum.Iron;
				case "bronze":
					return TierEnum.Bronze;
				case "silver":
					return TierEnum.Silver;
				case "gold":
					return TierEnum.Gold;
				case "platinum":
					return TierEnum.Platinum;
				case "diamond":
					return TierEnum.Diamond;
				case "master":
					return TierEnum.Master;
				case "grandmaster":
					return TierEnum.GrandMaster;
				case "challenger":
					return TierEnum.Challenger;
				default:
					return TierEnum.Unranked;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

		}
	}
}
