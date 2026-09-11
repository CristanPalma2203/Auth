using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers
{
    /// <summary>
    /// Target of ExceptionHandlingPath. Must exist so unhandled errors stay HTTP 500 JSON
    /// instead of being remapped to an empty 404 by a missing /Error endpoint.
    /// Re-execute keeps the original HTTP method, so this action accepts all verbs.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("/Error")]
        [AcceptVerbs("GET", "HEAD", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var error = feature?.Error;
            if (error != null)
            {
                logger.LogError(error, "Unhandled exception on {Path}", feature.Path);
            }

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "Error interno del servidor" });
        }
    }
}
