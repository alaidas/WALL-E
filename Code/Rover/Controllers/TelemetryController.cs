using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WALLE.Link;

namespace WALLE.Rover.Controllers
{
    [Route("api/[controller]")]
    public class TelemetryController : Controller
    {
        private readonly ILogger<TelemetryController> _logger;
        private readonly ConcurrentQueue<string> _events = new ConcurrentQueue<string>();

        public TelemetryController(ILinkClient linkClient, ILogger<TelemetryController> logger)
        {
            _logger = logger;
            _events.Enqueue("sdfsdfds");
        }

        [HttpGet("Event")]
        public async Task<IActionResult> GetEvent()
        {
            if (_events.TryDequeue(out string result))
                return Content(result, "text/html", Encoding.UTF8);

            return StatusCode((int)HttpStatusCode.NoContent);
        }

    }
}
