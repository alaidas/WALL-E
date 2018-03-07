using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WALLE.Link.Dto;
using WALLE.Link.Utils;

namespace WALLE.Link
{
    public class LinkClientHttp : ILinkClient
    {
        private readonly ILogger<LinkClientHttp> _logger;

        public LinkClientHttp(ILogger<LinkClientHttp> logger)
        {
            _logger = logger;
        }

        public async Task SendEventAsync(Event @event)
        {
            string json = JsonConvert.SerializeObject(@event);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "SharedAccessSignature sr=https%3A%2F%2FWALL-E.servicebus.windows.net%2Ftelemetry&sig=0Uihmcb8uD7FwQN0agIkuxRZIal8G5XrIFFFngWHqyE%3D&se=1520429951&skn=SendListen");
                HttpResponseMessage result = await client.PostAsync("https://WALL-E.servicebus.windows.net/telemetry/messages", new StringContent(json, Encoding.UTF8, "application/json"));

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
                        client.DefaultRequestHeaders.Add("Authorization", "SharedAccessSignature sr=https%3A%2F%2FWALL-E.servicebus.windows.net%2Ftelemetry&sig=rL1DaUwqRg14Oo7YDdtkDel98GmlKI6D%2B5ueJf8yzxk%3D&se=1520420493&skn=SendListen");
                        client.DefaultRequestHeaders.Add("Accept", "application/json");

                        while (!cancelToken.IsCancellationRequested)
                        {
                            HttpResponseMessage response = await client.DeleteAsync("https://WALL-E.servicebus.windows.net/telemetry/subscriptions/monitoring/messages/head/?api-version=2015-01", cancelToken.Token);
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
                    while(!cancelToken.IsCancellationRequested)
                    {
                        if (events.TryDequeue(out Event @event))
                            onEvent(@event);

                        await Task.Delay(50, cancelToken.Token);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, ex.Message);

                    if (!cancelToken.IsCancellationRequested)
                        cancelToken.Cancel();
                }
            }, cancelToken.Token);

            return result;
        }
    }
}
