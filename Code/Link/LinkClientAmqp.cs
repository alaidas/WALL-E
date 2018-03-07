using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace WALLE.Link
{
    public class LinkClientAmqp 
    {
        private const string TelemetryConnectionString = "Endpoint=sb://wall-e.servicebus.windows.net:80/;SharedAccessKeyName=RootTelemetry;SharedAccessKey=jZCvcx9TSfpAoorONU533EdPZk1MArGo1sEccmdv0oc=;EntityPath=telemetry";

        public async Task SendEventAsync<TEvent>(TEvent @event)
        {
            var topicClient = new TopicClient(new ServiceBusConnectionStringBuilder(TelemetryConnectionString));

            string json = JsonConvert.SerializeObject(@event);
            await topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes(json)));
        }
    }
}
