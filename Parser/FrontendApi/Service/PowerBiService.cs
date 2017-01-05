using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FrontendApi.Models;
using Newtonsoft.Json;

namespace FrontendApi.Service
{
    public class PowerBiService : IPowerBiService
    {
        private const string Resource = "https://analysis.windows.net/powerbi/api";
        private const string BaseUri = "https://api.powerbi.com/v1.0/myorg/";
        private readonly string _tokenEndpointUri;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public PowerBiService(string tenantId, string userName, string password, string clientId, string clientSecret)
        {
            _userName = userName;
            _password = password;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _tokenEndpointUri = $"https://login.microsoftonline.com/{tenantId}/oauth2/token";
        }

        public PBIDashboards GetDashboards(string accessToken)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create($"{BaseUri}dashboards") as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
           
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<PBIDashboards>(responseContent);
                }
            }
        }

        public PBIReports GetReports(string accessToken)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create($"{BaseUri}reports") as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<PBIReports>(responseContent);
                }
            }
        }

        public async Task<AzureAdTokenResponse> GetToken()
        {
            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", _userName),
                    new KeyValuePair<string, string>("password", _password),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret),
                    new KeyValuePair<string, string>("resource", Resource)
                }
            );
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync(_tokenEndpointUri, content);
                var json = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AzureAdTokenResponse>(json);
            }
        }
    }
}