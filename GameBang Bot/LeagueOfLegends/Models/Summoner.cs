using Newtonsoft.Json;

namespace GameBang_Bot.LeagueOfLegends.Models {
	class Summoner {
		[JsonProperty(PropertyName = "accountId")]
		public string AccountId { get; set; }
		[JsonProperty(PropertyName = "profileIconId")]
		public int ProfileIconId { get; set; }
		[JsonProperty(PropertyName = "revisionDate")]
		public long RevisionDate { get; set; }
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }
		[JsonProperty(PropertyName = "puuid")]
		public string Puuid { get; set; }
		[JsonProperty(PropertyName = "summonerLevel")]
		public long SummonerLevel { get; set; }
	}
}
