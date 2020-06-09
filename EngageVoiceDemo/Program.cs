using System;
using System.Threading.Tasks;
using RingCentral;

namespace EngageVoiceDemo
{
    class Program
    {
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
                    await rc.Revoke();
                }
            }).GetAwaiter().GetResult();
        }
    }
}