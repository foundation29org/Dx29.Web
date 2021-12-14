using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dx29.Data
{
    public class TimelineReport
    {
        public Dictionary<string, TimelineReportInfo> DictionaryTimeline { get; set; }
        public List<SymptomTimelineItem> listSymptomsNullInfo { get; set; }
    }
    public class TimelineReportInfo : Dictionary<string, IList<SymptomTimelineItem>> {}
}
