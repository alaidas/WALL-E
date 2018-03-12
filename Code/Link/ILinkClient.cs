using System;
using System.Threading;
using System.Threading.Tasks;
using WALLE.Link.Dto;

namespace WALLE.Link
{
    public interface ILinkClient
    {
        Task SendEventAsync(Event @event, CancellationToken cancellationToken);

        IDisposable SubscribeForTelemetry(string name, Action<Event> onEvent);

        IDisposable SubscribeForCommands(Action<Event> onEvent);
    }
}
