using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WALLE.Link;

namespace WALLE.Rover.Controllers
{
    [Route("api/[controller]")]
    public class TelemetryController : Controller
    {
        private readonly ILogger<TelemetryController> _logger;
        private readonly ILinkClient _linkClient;

        public TelemetryController(ILinkClient linkClient, ILogger<TelemetryController> logger)
        {
            _logger = logger;
            _linkClient = linkClient;
        }

        [HttpPost]
        public async Task<IActionResult> PostEvent([FromBody]string message)
        {
            await _linkClient.SendEventAsync(new Link.Dto.Event
            {
                Id = Guid.NewGuid().ToString(),
                CreationTime = DateTime.UtcNow,
                Sender = nameof(Rover),
                Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { Message = message }))
            }, CancellationToken.None);

            return NoContent();
        }
    }
}
