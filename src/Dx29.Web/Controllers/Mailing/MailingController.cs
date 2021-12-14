using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Dx29.Web.Services;
using Dx29.Data;

namespace Dx29.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class MailingController : ControllerBase
    {
        public MailingController(MailingService mailingService)
        {
            MailingService = mailingService;
        }

        public MailingService MailingService { get; }

        [HttpPost("sendEmail")]
        public async Task<IActionResult> SendEmailAsync([FromBody] MailBody mailBody)
        {
            try
            {
                var items = await MailingService.SendEmail(mailBody.userEmail, mailBody.subject, mailBody.bodyEmail, mailBody.language);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("sendEmailSupport")]
        public async Task<IActionResult> SendEmailSupportAsync([FromBody] MailBody mailBody)
        {
            try
            {
                var items = await MailingService.SendSupportEmail(mailBody.userEmail, mailBody.subject, mailBody.bodyEmail, mailBody.language);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }  
}
