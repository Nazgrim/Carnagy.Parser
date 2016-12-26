using Newtonsoft.Json;

namespace FrontendApi.Models
{
    public class AzureAdTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        public string expires_in { get; set; }
        public string ext_expires_in { get; set; }
        public string expires_on { get; set; }
        public string not_before { get; set; }
        public string resource { get; set; }
        public string refresh_token { get; set; }
    }
}