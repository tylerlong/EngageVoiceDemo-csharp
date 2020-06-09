using System;
using System.Net.Http;
using System.Threading.Tasks;
using RingCentral;

namespace EngageVoiceDemo
{
    class Program
    {
        public static HttpClient httpClient = new HttpClient();
        
        static void Main(string[] args)
        {
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
                    Console.WriteLine(rc.token.access_token);
                    
                    var dict = new System.Collections.Generic.Dictionary<string, string>();
                    dict.Add("rcAccessToken", rc.token.access_token);
                    dict.Add("rcTokenType", "Bearer");
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("https://engage.ringcentral.com/api/auth/login/rc/accesstoken"),
                        Content = new FormUrlEncodedContent(dict)
                    };
                    var r = await httpClient.SendAsync(httpRequestMessage);
                    Console.WriteLine(await r.Content.ReadAsStringAsync());
                    
                    await rc.Revoke();
                }
            }).GetAwaiter().GetResult();
        }
    }
}