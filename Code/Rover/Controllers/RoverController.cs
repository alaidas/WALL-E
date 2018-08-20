using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WALLE.Link;
using WALLE.Link.Utils;

namespace WALLE.Rover.Controllers
{
    [Route("api/[controller]")]
    public class RoverController : Controller
    {
        private readonly ILogger<RoverController> _logger;
        private readonly ILinkClient _linkClient;

        public RoverController(ILinkClient linkClient, ILogger<RoverController> logger)
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
                Content = JsonConvert.SerializeObject(new { Message = message })
            }, CancellationToken.None);

            return NoContent();
        }

        [HttpGet("CreateSasToken")]
        public IActionResult CreateSasToken(string url, string keyName, string key)
        {
            try
            {
                Ensure.IsNotNull(url, nameof(url));
                Ensure.IsNotNull(keyName, nameof(keyName));
                Ensure.IsNotNull(key, nameof(key));

                string token = SasTokenGenerator.CreateSasToken(url, keyName, key);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ex.Message });
            }
        }
    }
}
