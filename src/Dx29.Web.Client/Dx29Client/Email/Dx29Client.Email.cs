using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dx29.Data;
using Dx29.Services;
using Dx29.Web.Services;

namespace Dx29.Web
{
    public partial class Dx29Client
    {
        public async Task<MailingResponse> SendEmailSupportAsync(string subject,string bodyEmail)
        {
            MailBody mailBody = new MailBody
            {
                userEmail = "support@domain.org",
                subject = subject,
                bodyEmail = bodyEmail,
                language = Language
            };
            return await HttpClient.POSTAsync<MailingResponse>($"Mailing/sendEmailSupport", mailBody);
        }

        public async Task<MailingResponse> SendEmailAsync(string subject, string bodyEmail, string userEmail)
        {
            MailBody mailBody = new MailBody
            {
                userEmail = userEmail,
                subject = subject,
                bodyEmail = bodyEmail,
                language = Language
            };
            return await HttpClient.POSTAsync<MailingResponse>($"Mailing/sendEmail", mailBody);
        }
    }
}
