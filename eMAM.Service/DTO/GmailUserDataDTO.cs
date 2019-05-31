using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace eMAM.Service.DTO
{
    public class GmailUserDataDTO
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresInSec { get; set; }
    }
}
