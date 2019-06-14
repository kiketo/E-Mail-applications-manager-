using Newtonsoft.Json;
using System.Collections.Generic;

namespace eMAM.Service.DTO
{

    public class GmailMessagesListDTO
    {
        [JsonProperty("messages")]
        public List<MessageJson> Messages { get; set; }

        [JsonProperty("resultSizeEstimate")]
        public int Count { get; set; }
    }

    public class MessageJson
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
