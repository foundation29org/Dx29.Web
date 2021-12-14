using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dx29.Data
{
    public class TimelineInfo
    {
        public TimelineInfo()
        {
            Boxes = new List<TimelineInfoBox>();
        }
        public DateTimeOffset? Date { get; set; }

        public string Header => Date == null ? "null" : Date.Value.ToString("MMMM yyyy");


        public List<TimelineInfoBox> Boxes { get; set; }
    }

    public class TimelineInfoBox
    {
        public TimelineInfoBox()
        {
            Items = new List<SymptomTimelineItem>();
        }

        public DateTimeOffset? Date { get; set; }

        public string Subheader => Date == null ? "" : Date.Value.ToString("dd MMMM yyyy");

        public IList<SymptomTimelineItem> Items { get; set; }
    }


}
