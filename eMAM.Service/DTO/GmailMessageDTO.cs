using Newtonsoft.Json;
using System.Collections.Generic;

namespace eMAM.Service.DTO
{
    public class GmailMessageDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }

        [JsonProperty("internalDate")]
        public long InternalDate { get; set; }

        public string BodyAsString { get; set; }
    }
    public class Payload
    {
        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("headers")]
        public List<Header> Headers { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("parts")]
        public List<Part> Parts { get; set; }
    }
    public class Part
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("parts")]
        public List<Part> Parts { get; set; }

        [JsonProperty("body")]
        public Body Body { get; set; }
    }
    public class Body
    {
        [JsonProperty("size")]
        public int SizeInBytes { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }

    public class Header
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

}
