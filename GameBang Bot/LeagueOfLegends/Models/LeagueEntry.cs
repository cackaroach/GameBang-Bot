using GameBang_Bot.LeagueOfLegends.Enums;
using Newtonsoft.Json;

namespace GameBang_Bot.LeagueOfLegends.Models {
	class LeagueEntry {
		[JsonProperty(PropertyName = "leagueId")]
		public string LeagueId { get; set; }
		[JsonProperty(PropertyName = "summmonerId")]
		public string SummonerId { get; set; }
		[JsonProperty(PropertyName = "summonerName")]
		public string SummonerName { get; set; }
		[JsonProperty(PropertyName = "queueType")]
		[JsonConverter(typeof(Converters.QueueTypeConverter))]
		public QueueTypeEnum QueueType { get; set; }
		[JsonProperty(PropertyName = "tier")]
		[JsonConverter(typeof(Converters.TierConverter))]
		public TierEnum Tier { get; set; }
		[JsonProperty(PropertyName = "rank")]
		public string Rank { get; set; }
		[JsonProperty(PropertyName = "leaguePoints")]
		public int LeaguePoints { get; set; }
		[JsonProperty(PropertyName = "wins")]
		public int Wins { get; set; }
		[JsonProperty(PropertyName = "losses")]
		public int Losses { get; set; }
		[JsonProperty(PropertyName = "hotStreak")]
		public bool HotStreak { get; set; }
		[JsonProperty(PropertyName = "veteran")]
		public bool Veteran { get; set; }
		[JsonProperty(PropertyName = "freshBlood")]
		public bool FreshBlood { get; set; }
		[JsonProperty(PropertyName = "inactive")]
		public bool Inactive { get; set; }
	}
}
