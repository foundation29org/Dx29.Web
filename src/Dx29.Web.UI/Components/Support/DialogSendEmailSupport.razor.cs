using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dx29.Web.UI.Components
{
    partial class DialogSendEmailSupport
    {

        private string BuildBodySupportEmail()
        {
            string bodyEmail = "";
            bodyEmail += "<div> <p>" + Localize["The user: "] + "<a href = \"mailto:" + Model.EmailContact + "\">" + Model.EmailContact + "</a>" + Localize[" , requests to contact the support team of Foundation29 to obtain information about "] + Subject + ".<p> </div> </br> <div> <p>" + Localize["The data related to his query is the following:"] + "</p></div></br>";

            bodyEmail += " <div> <ul> ";
            foreach (var property in Info)
            {
                bodyEmail += "<li> " + Localize[property.Key] + " : " + Info[property.Key] + " </li>";
            }
            if (Model.Message?.Length > 0)
            {
                bodyEmail += "<li>" + Localize["User message: "] + Model.Message + " </li>";
            }
            bodyEmail += " </ul> </div>";

            bodyEmail += " <div> <p>" + Localize["The request was made from the page: "] + Dx29Section + " </p></div>";

            return bodyEmail;
        }

        private string BuildBodyUserEmail()
        {
            string bodyEmail = " <div><img style=\"width: 17px; height: 29px;\" src=\"https://dx29.ai/assets/img/logo-Dx29.png\" /></div> </br>";
            bodyEmail += "<div> <p>" + Localize["You have made a request to the Foundation29 support team to obtain information about "] + Subject + Localize[", with the following data:"] + "</p></div></br>";
            
            bodyEmail += " <div> <ul> ";
            foreach (var property in Info)
            {
                bodyEmail += "<li> " + Localize[property.Key] + " : " + Info[property.Key] + " </li>";
            }
            if (Model.Message?.Length > 0)
            {
                bodyEmail += "<li>" + Localize["User message: "] + Model.Message + " </li>";
            }
            bodyEmail += " </ul> </div></br>";
            bodyEmail += "<div> <p>" + Localize["Thank you for trusting us. We will contact you shortly to help you with your question."]+"</p></div></br></br>";

            bodyEmail += "<div><p>" + Localize["Yours sincerely,"]+"</p></div>";
            bodyEmail += "<div><p>" + Localize["The support team of "]+ "<a href=\"https://domain.org/\">" + Localize["Foundation 29"] + "</a>" + "</p></div>";
            
            return bodyEmail;
        }
    }
}
