using GameBang_Bot.LeagueOfLegends.Models;
using GameBang_Bot.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameBang_Bot.LeagueOfLegends {
	class LolParser : IDisposable {
		private HttpClient client = null;

		public LolParser() {
			client = new HttpClient();
		}

		public async Task<Summoner> ParseSummonerAsync(string summonerName) {
			var response = await client.GetAsync($"https://kr.api.riotgames.com/lol/summoner/v4/summoners/by-name/{ToUTF8(summonerName)}?api_key={PropertiesContext.LeagueOfLegends.Token}");
			var json = await response.Content.ReadAsStringAsync();

			switch (response.StatusCode) {
				case System.Net.HttpStatusCode.OK:
					return JsonConvert.DeserializeObject<Summoner>(json);
				default:
					var error = JsonConvert.DeserializeObject<Error>(json);
					throw new Exception($"{error.Status.StatusCode}: {error.Status.Message}");
			}
		}

		public async Task<List<LeagueEntry>> ParseLeagueEntryListAsync(string summonerId) {
			var response = await client.GetAsync($"https://kr.api.riotgames.com/lol/league/v4/entries/by-summoner/{summonerId}?api_key={PropertiesContext.LeagueOfLegends.Token}");

			switch (response.StatusCode) {
				case System.Net.HttpStatusCode.OK:
					var json = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<List<LeagueEntry>>(json);
				default:
					return null;
			}
		}

		public virtual void Dispose() {
			client.Dispose();
		}

		private static string ToUTF8(string str) {
			StringBuilder sb = new StringBuilder();
			byte[] bytes = Encoding.UTF8.GetBytes(str);

			foreach (byte b in bytes)
				sb.Append(string.Format("%{0:X}", b));

			return sb.ToString();
		}
	}
}
