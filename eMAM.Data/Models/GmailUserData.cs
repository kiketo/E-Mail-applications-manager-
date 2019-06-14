using Newtonsoft.Json;
using System;

namespace eMAM.Data.Models
{
    public class GmailUserData
    {
        public int Id { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}

