using System;
using System.Collections.Concurrent;
using System.Globalization;
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
            _sasToken = GetSasToken(config["Url"], config["KeyName"], config["Key"]);
            Console.WriteLine($"SAS: {_sasToken}");

            _logger = logger;
        }

        public async Task SendEventAsync(Event @event, CancellationToken cancellationToken)
        {
            Ensure.IsNotNull(@event, nameof(@event));
            Ensure.IsNotNull(@event.ContentType, nameof(@event.ContentType));
            Ensure.IsNotNull(@event.Id, nameof(@event.Id));
            Ensure.IsNotNull(@event.Sender, nameof(@event.Sender));
            Ensure.IsNotNull(@event.Content, nameof(@event.Content));
            Ensure.IsMoreThan(@event.CreationTime, DateTime.UtcNow.Date, nameof(@event.CreationTime));

            string json = JsonConvert.SerializeObject(@event);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", _sasToken);
                HttpResponseMessage result = await client.PostAsync($"{_telemetryUrl}/messages", new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken);

                result.EnsureSuccessStatusCode();
            }
        }

        public IDisposable SubscribeForTelemetry(string name, Action<Event> onEvent)
        {
            Ensure.IsNotNull(name, nameof(name));

            return SubscribeForEvents($"{_telemetryUrl}/subscriptions/{name}/messages/head/?api-version=2015-01", onEvent);
        }

        public IDisposable SubscribeForCommands(Action<Event> onEvent)
        {
            return SubscribeForEvents($"{_commandsUrl}/messages/head/?api-version=2015-01", onEvent);
        }

        private IDisposable SubscribeForEvents(string url, Action<Event> onEvent)
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
                            try
                            {
                                HttpResponseMessage response = await client.DeleteAsync(url, cancelToken.Token);
                                if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    await Task.Delay(100, cancelToken.Token);
                                    continue;
                                }

                                string json = await response.Content.ReadAsStringAsync();
                                var @event = JsonConvert.DeserializeObject<Event>(json);

                                events.Enqueue(@event);
                            }
                            catch(Exception ex)
                            {
                                _logger.LogError(ex, ex.Message);
                            }
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
                        try
                        {
                            if (events.TryDequeue(out Event @event))
                                onEvent(@event);

                            await Task.Delay(50, cancelToken.Token);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
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

            return result;
        } 

        public string GetSasToken(string url, string keyName, string key)
        {
            return SasTokenGenerator.CreateSasToken(url, keyName, key);
        }
    }
}
