using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    public class MailingService
    {
        const string FROM_EMAIL = "support@domain.org";
        const string FROM_EMAIL_DEV = "dev@domain.org";
        const string BCC_EMAIL = "support@domain.org";
        const string BCC_EMAIL_DEV = "dev@domain.org";

        public MailingService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<MailingResponse> SendEmail(string userEmail, string subject, string bodyEmail, string language)
        {
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            string _bodyEmail = @bodyEmail.Replace("\r\n", "<br>").Replace("\"", "");
            MailingResponse response = await HttpClient.POSTAsync<MailingResponse>($"sendemail?To={userEmail}&From={FROM_EMAIL}&Subject={subject}&BCC={BCC_EMAIL}", new StringContent(_bodyEmail, Encoding.UTF8, "text/html"));
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return response;
        }

        public async Task<MailingResponse> SendSupportEmail(string userEmail, string subject, string bodyEmail, string language)
        {
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            string _bodyEmail = @bodyEmail.Replace("\r\n", "<br>").Replace("\"", "");
            MailingResponse response = await HttpClient.POSTAsync<MailingResponse>($"sendemail?To={userEmail}&From={FROM_EMAIL_DEV}&Subject={subject}&BCC={BCC_EMAIL_DEV}", new StringContent(_bodyEmail, Encoding.UTF8, "text/html"));
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return response;
        }
    }
}
