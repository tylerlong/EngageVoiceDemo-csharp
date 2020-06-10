using System;
using System.Net.Http;
using System.Threading.Tasks;
using dotenv.net;
using RingCentral;
using Newtonsoft.Json;

namespace EngageVoiceDemo
{
    class EngageVoiceToken
    {
        public string refreshToken;
        public string accessToken;
        public string tokenType;
        public string platformId;
    }
    
    class Program
    {
        public static HttpClient httpClient = new HttpClient();
        public static string engageVoiceServer = Environment.GetEnvironmentVariable("ENGAGE_VOICE_SERVER_URL");
        
        static void Main(string[] args)
        {
            DotEnv.Config(true);
            
            Task.Run(async () =>
            {
                using (var rc = new RestClient(
                    Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_ID"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_CLIENT_SECRET"),
                    Environment.GetEnvironmentVariable("RINGCENTRAL_SERVER_URL")
                ))
                {
                    await rc.Authorize(
                        Environment.GetEnvironmentVariable("RINGCENTRAL_USERNAME"),
                        Environment.GetEnvironmentVariable("RINGCENTRAL_EXTENSION"),
                        Environment.GetEnvironmentVariable("RINGCENTRAL_PASSWORD")
                    );
                    
                    var dict = new System.Collections.Generic.Dictionary<string, string>();
                    dict.Add("rcAccessToken", rc.token.access_token);
                    dict.Add("rcTokenType", "Bearer");
                    var uriBuilder = new UriBuilder($"{engageVoiceServer}/api/auth/login/rc/accesstoken");
                    // uriBuilder.Query = $"rcAccessToken={rc.token.access_token}&rcTokenType=Bearer";
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = uriBuilder.Uri,
                        Content = new FormUrlEncodedContent(dict)
                    };
                    var r = await httpClient.SendAsync(httpRequestMessage);
                    var engageVoiceToken =
                        JsonConvert.DeserializeObject<EngageVoiceToken>(await r.Content.ReadAsStringAsync());

                    r = await httpClient.SendAsync(new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        Headers = { {"Authorization", $"Bearer {engageVoiceToken.accessToken}"} },
                        RequestUri = new Uri($"{engageVoiceServer}/voice/api/v1/admin/accounts")
                    });
                    Console.WriteLine(await r.Content.ReadAsStringAsync());
                    
                    await rc.Revoke();
                }
            }).GetAwaiter().GetResult();
        }
    }
}