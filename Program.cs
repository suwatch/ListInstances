using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ListInstances
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Usage: ListInstances.exe scmHostName");
                    return;
                }

                // bypass in testing env
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var hostName = args[0];
                var path = "/operations/listinstances";
                var requestId = $"{Guid.NewGuid()}";
                await PostAsync(hostName, path, requestId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static async Task PostAsync(string hostName, string path, string requestId)
        {
            using (var client = new HttpClient())
            {
                var address = $"https://{hostName}{path}";
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("logic-app", "1.0"));
                client.DefaultRequestHeaders.Add(Constants.SiteRestrictedToken, SimpleWebTokenHelper.CreateToken(DateTime.UtcNow.AddMinutes(5)));
                client.DefaultRequestHeaders.Add(Constants.RequestIdHeader, requestId);

                Console.Write($"Post {address} ... ");
                var payload = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(address, payload))
                {
                    Console.WriteLine($"{response.StatusCode}");
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response: {content}");
                }
            }
        }
    }
}
