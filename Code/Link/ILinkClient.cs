using System;
using System.Threading.Tasks;
using WALLE.Link.Dto;

namespace WALLE.Link
{
    public interface ILinkClient
    {
        Task SendEventAsync(Event @event);

        IDisposable SubscribeForEvents(string name, Action<Event> onEvent);
    }
}
