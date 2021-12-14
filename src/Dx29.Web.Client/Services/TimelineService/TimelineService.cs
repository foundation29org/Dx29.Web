using System;
using System.Linq;
using System.Collections.Generic;

using Dx29.Data;

namespace Dx29.Web.Services
{
    public partial class TimelineService
    {
        public TimelineService()
        {
        }

        public IList<SymptomTimelineItem> TrasformForm(SymptomTimeline symptomTimeline)
        {
            return symptomTimeline.Items;
        }

        public IList<TimelineInfo> TransformTimeline(SymptomTimeline symptomTimeline)
        {
            var timelines = new List<TimelineInfo>();
            var timelineItems = symptomTimeline.Items;
            var groupYYMM = symptomTimeline.Items.Where(r => r.StartDate != null).GroupBy(r => r.StartDate.Value.ToString("yyMM"));
            foreach (var yymm in groupYYMM.OrderBy(r => r.Key))
            {
                var lineInfo = new TimelineInfo
                {
                    Date = yymm.First().StartDate
                };
                var groupDD = yymm.GroupBy(r => r.StartDate.Value.ToString("dd"));
                foreach (var dd in groupDD.OrderBy(r => r.Key))
                {
                    var box = new TimelineInfoBox
                    {
                        Date = dd.First().StartDate
                    };
                    box.Items = GetItemsInBox(timelineItems, box.Date.Value);
                    lineInfo.Boxes.Add(box);
                }
                timelines.Add(lineInfo);
            }
            return timelines;
        }

        private IList<SymptomTimelineItem> GetItemsInBox(IList<SymptomTimelineItem> timelineItems, DateTimeOffset date)
        {
            return timelineItems.Where(r => r.StartDate <= date && (r.EndDate == null || r.EndDate >= date)).ToArray();
        }

        public IList<TimelineInfo> TransformTimelineOLD(SymptomTimeline symptomTimeline)
        {
            var timelineList = new List<TimelineInfo>();
            var timelineListOrdered = new List<TimelineInfo>();
            foreach (var item in symptomTimeline.Items.Where(r => r.StartDate != null).OrderBy(x => x.StartDate).ToList())
            {
                if (item.StartDate != null)
                {
                    var timeline = new TimelineInfo { Date = item.StartDate };
                    var timelineBox = new TimelineInfoBox { Date = item.StartDate };
                    timelineBox.Items.Add(item);
                    foreach (var item2 in symptomTimeline.Items)
                    {
                        if (item2.Id != item.Id)
                        {
                            if (item2.StartDate != null)
                            {
                                if (item2.EndDate == null)
                                {
                                    var compare = DateTimeOffset.Compare((DateTimeOffset)item.StartDate, (DateTimeOffset)item2.StartDate);
                                    if (compare >= 0)
                                    {
                                        timelineBox.Items.Add(item2);
                                    }
                                }
                                else
                                {
                                    var compare1 = DateTimeOffset.Compare((DateTimeOffset)item.StartDate, (DateTimeOffset)item2.EndDate);
                                    var compare2 = DateTimeOffset.Compare((DateTimeOffset)item.StartDate, (DateTimeOffset)item2.StartDate);
                                    if ((compare1 <= 0) && (compare2 >= 0))
                                    {
                                        timelineBox.Items.Add(item2);
                                    }
                                }
                            }
                        }
                    }
                    timeline.Boxes.Add(timelineBox);
                    timelineList.Add(timeline);
                }
            }
            foreach (var timeline in timelineList.GroupBy(r => DateTimeOffset.Parse(r.Date.Value.ToString("yyyy/MM/01"))))
            {
                var newHeader = timeline.Key;
                var timelineOrder = new TimelineInfo { Date = newHeader };
                List<TimelineInfo> timelineBoxesList = timeline.ToList();
                foreach (var item in timelineBoxesList)
                {
                    foreach (var box in item.Boxes)
                    {
                        var timelineOrderBox = new TimelineInfoBox { Date = box.Date };
                        foreach (var symptom in box.Items)
                        {
                            timelineOrderBox.Items.Add(symptom);
                        }
                        timelineOrder.Boxes.Add(timelineOrderBox);
                    }

                }
                timelineListOrdered.Add(timelineOrder);
            }
            return timelineListOrdered;
        }

        public IList<TimelineInfo> TransformTimelineNull(SymptomTimeline symptomTimeline)
        {
            var timelineList = new List<TimelineInfo>();
            if (symptomTimeline.Items.Where(r => r.StartDate == null).ToList().Count > 0)
            {
                var timeline = new TimelineInfo { Date = null };
                var timelineBox = new TimelineInfoBox { Date = null };
                foreach (var item in symptomTimeline.Items)
                {
                    if (item.StartDate == null)
                    {
                        timelineBox.Items.Add(item);
                    }
                }
                timeline.Boxes.Add(timelineBox);
                timelineList.Add(timeline);
            }
            return timelineList;
        }

        public SymptomTimeline TransformFormToSymptomTimeline(IList<SymptomTimelineItem> form)
        {
            SymptomTimeline symptomTimeline = new SymptomTimeline();
            symptomTimeline.Items = form;
            return symptomTimeline;
        }

        public SymptomTimeline UpdateIsCurrentStatus(SymptomTimeline symptomTimeline)
        {
            foreach (var item in symptomTimeline.Items.ToList())
            {
                if ((item.StartDate != null) && (item.EndDate == null))
                {
                    item.IsCurrent = true;
                }
                else
                {
                    item.IsCurrent = false;
                }
            }
            return symptomTimeline;
        }

        public TimelineReport TransformTimelineReport(SymptomTimeline symptomTimeline)
        {
            TimelineReport timelineReport = new TimelineReport { listSymptomsNullInfo = symptomTimeline.Items.Where(r => r.StartDate == null).ToList() };

            IList<TimelineInfo> listTimelineInfo = TransformTimeline(symptomTimeline);
            timelineReport.DictionaryTimeline = new Dictionary<string, TimelineReportInfo>();
            foreach (var item in listTimelineInfo)
            {
                var reportInfo = new TimelineReportInfo();
                foreach (var subitem in item.Boxes)
                {
                    reportInfo.Add(subitem.Subheader, subitem.Items);
                }
                timelineReport.DictionaryTimeline.Add(item.Header, reportInfo);
            }
            return timelineReport;
        }
    }
}
