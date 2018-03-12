using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WALLE.Link.Dto;
using WALLE.Link.Utils;

namespace WALLE.Link
{
    public class LinkClientHttp : ILinkClient
    {
        private readonly ILogger<LinkClientHttp> _logger;
        private readonly string _telemetryUrl;
        private readonly string _commandsUrl;
        private readonly string _sasToken;

        public LinkClientHttp(IConfiguration configuration, ILogger<LinkClientHttp> logger)
        {
            IConfigurationSection config = configuration.GetSection("Link").GetSection("Http");
            _telemetryUrl = config["TelemetryUrl"];
            _commandsUrl = config["CommandsUrl"];

            config = config.GetSection("SAS");
            _sasToken = CreateSasToken(config["Url"], config["KeyName"], config["Key"]);
            Console.WriteLine($"SAS: {_sasToken}");

            _logger = logger;
        }

        public async Task SendEventAsync(Event @event)
        {
            string json = JsonConvert.SerializeObject(@event);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", _sasToken);
                HttpResponseMessage result = await client.PostAsync(_telemetryUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                result.EnsureSuccessStatusCode();
            }
        }

        public IDisposable SubscribeForEvents(string name, Action<Event> onEvent)
        {
            var cancelToken = new CancellationTokenSource();
            IDisposable result = new DisposableCancellationToken(cancelToken);

            var events = new ConcurrentQueue<Event>();

            Task.Run(async () =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", _sasToken);
                        client.DefaultRequestHeaders.Add("Accept", "application/json");

                        while (!cancelToken.IsCancellationRequested)
                        {
                            HttpResponseMessage response = await client.DeleteAsync(_commandsUrl, cancelToken.Token);
                            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                await Task.Delay(100, cancelToken.Token);
                                continue;
                            }

                            string json = await response.Content.ReadAsStringAsync();
                            var @event = JsonConvert.DeserializeObject<Event>(json);

                            events.Enqueue(@event);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);

                    if (!cancelToken.IsCancellationRequested)
                        cancelToken.Cancel();
                }
            }, cancelToken.Token);


            Task.Run(async () =>
            {
                try
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        if (events.TryDequeue(out Event @event))
                            onEvent(@event);

                        await Task.Delay(50, cancelToken.Token);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);

                    if (!cancelToken.IsCancellationRequested)
                        cancelToken.Cancel();
                }
            }, cancelToken.Token);

            return result;
        }

        private static string CreateSasToken(string resourceUri, string keyName, string key)
        {
            TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            const long tenYears = 60 * 60 * 24 * 356 * 10;

            string expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + tenYears);
            string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            return string.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);
        }
    }
}
