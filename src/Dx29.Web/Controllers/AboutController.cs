using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dx29.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AboutController : ControllerBase
    {
        [HttpGet("version")]
        public IActionResult Version()
        {
            return Redirect("/api/v1/About/version");
        }

        [HttpGet("/api/v1/[controller]/version")]
        public IActionResult VersionV1()
        {
            return Ok(Startup.VERSION);
        }
    }
}
