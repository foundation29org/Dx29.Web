using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Dx29.Web.Services
{
    public class EmailHelper
    {
        public EmailHelper(MailingService mailingService, DocumentsService documentsService)
        {
            MailingService = mailingService;
            DocumentsService = documentsService;
        }

        public MailingService MailingService { get; set; }
        public DocumentsService DocumentsService { get; set; }


        public async Task<bool> SendConfirmationEmailAsync(string userEmail, string language, string confirmationLink)
        {
            if (IsSampleUser(userEmail))
            {
                return true;
            }

            try
            {
                string subject = "Activate%20account";
                if (language.ToLower().StartsWith("es"))
                {
                    subject = "Activar%20cuenta";
                }

                string bodyDocument = await DocumentsService.Download("Email", "ActivateAccount.txt", language.Split("-")[0], null);
                string bodyEmail = bodyDocument.Replace("confirmationLink", confirmationLink);
                var response = await MailingService.SendEmail(userEmail, subject, bodyEmail, language);
                if (response.status != 202)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine("Exception...");
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> SendForgotPasswordEmailAsync(string userEmail, string language, string callbackUrl)
        {
            if (IsSampleUser(userEmail))
            {
                return true;
            }

            try
            {
                string subject = "Reset%20password";
                string bodyDocument = await DocumentsService.Download("Email", "SendForgotPassword.txt", language.Split("-")[0], null);
                string bodyEmail = bodyDocument.Replace("__callbackUrl__", HtmlEncoder.Default.Encode(callbackUrl));
                var response = await MailingService.SendEmail(userEmail, subject, bodyEmail, language);
                if (response.status != 202)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine("Exception...");
                Console.WriteLine(ex);
                return false;
            }
        }
        public async Task<bool> SendShareInternalAsync(string userName, string userEmail, string caseName, string userRole, string ownerEmail, string userRequestShareName, string userRequestShareEmail, string language, string message)
        {
            if (IsSampleUser(userRequestShareEmail))
            {
                return true;
            }
            try
            {
                string subject = userName + "%20has%20shared%20the%20case%20"+caseName+"%20with%20you";
                if (language.ToLower().StartsWith("es"))
                {
                    subject = userName + "%20te%20ha%20compartido%20el%20caso%20" + caseName;
                }
                string bodyDocument = await DocumentsService.Download("Email", "Share_internal.txt", language.Split("-")[0], null);
                string bodyEmail = replaceBodySharedEmails(bodyDocument, userName, userEmail, userRole, caseName, userRequestShareName, userRequestShareEmail, language, true, message);
                var response = await MailingService.SendEmail(userRequestShareEmail, subject, bodyEmail, language);
                if (response.status != 202)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine("Exception...");
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> SendShareExternalAsync(string userName, string userEmail, string caseName, string userRole, string ownerEmail, string userRequestShareName, string userRequestShareEmail, string language, string message)
        {
            if (IsSampleUser(userRequestShareEmail))
            {
                return true;
            }

            try
            {
                string subject = userName + "%20has%20shared%20a%20case%20with%20you%20in%20Dx29";
                if (language.ToLower().StartsWith("es"))
                {
                    subject = userName + "%20te%20ha%20compartido%20un%20caso%20en%20Dx29";
                }
                string bodyDocument = await DocumentsService.Download("Email", "Share_external.txt", language.Split("-")[0], null);
                string bodyEmail = replaceBodySharedEmails(bodyDocument, userName, userEmail, userRole, caseName, userRequestShareName, userRequestShareEmail, language, true, message);
                var response = await MailingService.SendEmail(userRequestShareEmail, subject, bodyEmail, language);
                if (response.status != 202)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine("Exception...");
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> SendUnShareInternalAsync(string userName, string userEmail, string caseName, string userRole, string ownerEmail, string userRequestShareName, string userRequestShareEmail, string language,bool includeMessage, string message)
        {
            if (IsSampleUser(userRequestShareEmail))
            {
                return true;
            }
            try
            {
                string subject = userName + "%20has%20stoppen%20sharing%20the%20case%20" + caseName + "%20with%20you";
                if (language.ToLower().StartsWith("es"))
                {
                    subject = userName + "%20ha%20dejado%20de%20compartir%20el%20caso" + caseName +"%20contigo.";
                }
                string bodyDocument = await DocumentsService.Download("Email", "Unshare_internal.txt", language.Split("-")[0], null);
                string bodyEmail = replaceBodySharedEmails(bodyDocument, userName, userEmail, userRole, caseName, userRequestShareName, userRequestShareEmail, language, includeMessage, message);
                var response = await MailingService.SendEmail(userRequestShareEmail, subject, bodyEmail, language);
                if (response.status != 202)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine("Exception...");
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> SendUnShareExternalAsync(string userName, string userEmail, string caseName, string userRole, string ownerEmail, string userRequestShareName, string userRequestShareEmail, string language,bool includeMessage, string message)
        {
            if (IsSampleUser(userRequestShareEmail))
            {
                return true;
            }
            try
            {
                string subject = userName + "%20has%20stopped%20sharing%20the%20case%20" + caseName + "%20with%20you";
                if (language.ToLower().StartsWith("es"))
                {
                    subject = userName + "%20ha%20dejado%20de%20compartir%20el%20caso" + caseName + "%20contigo.";
                }
                string bodyDocument = await DocumentsService.Download("Email", "Unshare_external.txt", language.Split("-")[0], null);
                string bodyEmail = replaceBodySharedEmails(bodyDocument, userName, userEmail, userRole, caseName, userRequestShareName, userRequestShareEmail, language, includeMessage, message);
                var response = await MailingService.SendEmail(userRequestShareEmail, subject, bodyEmail, language);
                if (response.status != 202)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine("Exception...");
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> SendRequestApprovalInternalAsync(string userName, string userEmail, string caseName, string userRole, string ownerEmail, string userRequestShareName, string userRequestShareEmail, string language, string message)
        {
            if (IsSampleUser(ownerEmail))
            {
                return true;
            }
            try
            {
                string subject = userName + "%20wants%20to%20share%20your%20case%20"+caseName;
                if (language.ToLower().StartsWith("es"))
                {
                    subject = userName + "%20quiere%20compartir%20tu%20caso%20" + caseName;
                }
                string bodyDocument = await DocumentsService.Download("Email", "RequestApproval_internal.txt", language.Split("-")[0], null);
                string bodyEmail = replaceBodySharedEmails(bodyDocument, userName, userEmail, userRole, caseName, userRequestShareName, userRequestShareEmail, language, true, message);
                var response = await MailingService.SendEmail(ownerEmail, subject, bodyEmail, language);
                if (response.status != 202)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine("Exception...");
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> SendRequestApprovalExternalAsync(string userName, string userEmail, string caseName, string userRole, string ownerEmail, string userRequestShareName, string userRequestShareEmail, string language, string message)
        {
            if (IsSampleUser(ownerEmail))
            {
                return true;
            }
            try
            {
                string subject = userName + "%20wants%20to%20share%your%20case%20" + caseName;
                if (language.ToLower().StartsWith("es"))
                {
                    subject = userName + "%20quiere%20compartir%20tu%20caso%20" + caseName;
                }
                string bodyDocument = await DocumentsService.Download("Email", "RequestApproval_external.txt", language.Split("-")[0], null);
                string bodyEmail = replaceBodySharedEmails(bodyDocument, userName, userEmail, userRole, caseName, userRequestShareName, userRequestShareEmail, language, true, message);
                var response = await MailingService.SendEmail(ownerEmail, subject, bodyEmail, language);
                if (response.status != 202)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine("Exception...");
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> SendInformationToOwnerInternalAsync(string userName, string userEmail, string caseName, string userRole, string ownerEmail, string userRequestShareName, string userRequestShareEmail, string language, string message)
        {
            if (IsSampleUser(ownerEmail))
            {
                return true;
            }
            try
            {
                string subject = userName + "%20has%20stopped%20sharing%20the%20case%20" + caseName + "%20with%20" + userRequestShareName;
                if (language.ToLower().StartsWith("es"))
                {
                    subject = userName + "%20ha%20dejado%20de%20compartir%20el%20caso" + caseName + "%20con%20" + userRequestShareName;
                }
                string bodyDocument = await DocumentsService.Download("Email", "SendInfoUnshared_internal.txt", language.Split("-")[0], null);
                string bodyEmail = replaceBodySharedEmails(bodyDocument, userName, userEmail, userRole, caseName, userRequestShareName, userRequestShareEmail, language, true, message);
                var response = await MailingService.SendEmail(ownerEmail, subject, bodyEmail, language);
                if (response.status != 202)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine("Exception...");
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> SendInformationToOwnerExternalAsync(string userName, string userEmail, string caseName, string userRole, string ownerEmail, string userRequestShareName, string userRequestShareEmail, string language, string message)
        {
            if (IsSampleUser(ownerEmail))
            {
                return true;
            }
            try
            {
                string subject = userName + "%20has%20stopped%20sharing%20the%20case%20" + caseName + "%20with%20"+ userRequestShareName;
                if (language.ToLower().StartsWith("es"))
                {
                    subject = userName + "%20ha%20dejado%20de%20compartir%20el%20caso" + caseName + "%20con%20"+ userRequestShareName;
                }
                string bodyDocument = await DocumentsService.Download("Email", "SendInfoUnshared_external.txt", language.Split("-")[0], null);
                string bodyEmail = replaceBodySharedEmails(bodyDocument, userName, userEmail, userRole, caseName, userRequestShareName, userRequestShareEmail, language,true, message);
                var response = await MailingService.SendEmail(ownerEmail, subject, bodyEmail, language);
                if (response.status != 202)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine("Exception...");
                Console.WriteLine(ex);
                return false;
            }
        }
        private string replaceBodySharedEmails(string bodyEmail, string userName, string userEmail, string userRole, string caseName, string userRequestShareName, string userRequestShareEmail, string language, bool includeMessage, string message)
        {
            string userRoleLocalize = userRole.ToLower();
            if (language.ToLower().StartsWith("es"))
            {
                if (userRoleLocalize == "physician")
                {
                    userRoleLocalize = "médico";
                }
                else if(userRoleLocalize == "patient")
                {
                    userRoleLocalize = "paciente";
                }
            }
            
            bodyEmail = bodyEmail.Replace("__userName__", userName);
            bodyEmail = bodyEmail.Replace("__userEmail__", userEmail);
            bodyEmail = bodyEmail.Replace("__caseName__", caseName);
            bodyEmail = bodyEmail.Replace("__userRequestShareName__", userRequestShareName);
            bodyEmail = bodyEmail.Replace("__userRequestShareEmail__", userRequestShareEmail);
            bodyEmail = bodyEmail.Replace("__userRole__", userRoleLocalize);
            if (includeMessage)
            {
                if (message != null)
                {
                    if (language.ToLower().StartsWith("es"))
                    {
                        bodyEmail = bodyEmail.Replace("__message__", "<div>Además, el " + userRoleLocalize + " ha añadido este mensaje: " + message+ "</div><br/>");
                    }
                    else
                    {
                        bodyEmail = bodyEmail.Replace("__message__", "<div>In adition, the " + userRoleLocalize + " add this message: " + message + "</div><br/>");
                    }
                }
                else
                {
                    bodyEmail = bodyEmail.Replace("__message__", "");
                }
            }
            else
            {
                bodyEmail = bodyEmail.Replace("__message__", "");
            }
            return bodyEmail;
        }
        private bool IsSampleUser(string userEmail)
        {
            return false;
        }        
    }
}
