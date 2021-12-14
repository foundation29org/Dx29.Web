using System;
using System.Collections.Generic;
using System.Linq;

namespace Dx29.Data
{
    public class DiagnosisMoreInfo: List<DiagnosisMoreInfoItems>
    {
        
    }
    public class DiagnosisMoreInfoItems
    {
        public DiagnosisMoreInfoItems()
        {
            items = new List<DiagnosisMoreInfo_Items>();
        }
        public string title { get; set; }
        public string content { get; set; }
        public List<DiagnosisMoreInfo_Items> items { get; set; }
        public List<string> urls { get; set; }
    }

    public class DiagnosisMoreInfo_Items
    {
        public string title { get; set; }
        public string content { get; set; }
    }

    public class DiagnosisMoreInfoList
    {
        public DiagnosisMoreInfoListQuery query { get; set; }
    }

    public class DiagnosisMoreInfoBody
    {
        public string text { get; set; }
        public string lang { get; set; }
    }

    public class DiagnosisMoreInfoListQuery
    {
        public List<DiagnosisMoreInfoListSeach> search { get; set; }
    }

    public class DiagnosisMoreInfoListSeach
    {
        public string title { get; set; }
        public string snippet { get; set; }
    }
}
