using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class MailBody
    {
        public string userEmail { get; set; }
        public string subject { get; set; }
        public string bodyEmail { get; set; }

        public string language { get; set; }
    }

    public class SupportInfoEmail:Dictionary<string,string>
    {
    }

}
