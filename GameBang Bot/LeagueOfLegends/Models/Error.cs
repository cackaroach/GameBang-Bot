using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBang_Bot.LeagueOfLegends.Models {
    public class Error {
        [JsonProperty(PropertyName = "status")]
        public Status Status { get; set; }
    }

    public class Status {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "status_code")]
        public int StatusCode { get; set; }
    }
}
