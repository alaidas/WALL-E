using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WALLE.Link.Utils;

namespace WALLE.Rover.Controllers
{
    [Route("api/[controller]")]
    public class UtilsController : Controller
    {
        private readonly ILogger<UtilsController> _logger;

        public UtilsController(ILogger<UtilsController> logger)
        {
            _logger = logger;
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