﻿using Newtonsoft.Json;

namespace AABillApi.Models
{
    public class TokenManagement : ITokenManagement
    {
        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }

        [JsonProperty("accessExpiration")]
        public int AccessExpiration { get; set; }

        [JsonProperty("refreshExpiration")]
        public int RefreshExpiration { get; set; }
    }

    public interface ITokenManagement
    {
        string Secret { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
        int AccessExpiration { get; set; }
        int RefreshExpiration { get; set; }
    }
}
