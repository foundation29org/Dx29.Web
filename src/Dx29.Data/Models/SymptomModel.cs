using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Dx29.Data;

namespace Dx29.Web.Models
{
    public class SymptomsModel
    {
        public SymptomsModel()
        {
            Sources = new Dictionary<string, SymptomSourceModel>(StringComparer.OrdinalIgnoreCase);
            Symptoms = new Dictionary<string, SymptomModel>(StringComparer.OrdinalIgnoreCase);
        }

        public Dictionary<string, SymptomSourceModel> Sources { get; set; }
        public Dictionary<string, SymptomModel> Symptoms { get; set; }
    }

    public class SymptomModel
    {
        public SymptomModel()
        {
            Sources = new HashSet<string>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public IList<Reference> Categories { get; set; }
        public string Status { get; set; }

        [JsonIgnore]
        public bool IsSelected
        {
            get => Status.ToLower() == "selected";
            set
            {
                Status = value ? "Selected" : "Unselected";
                UpdatedOn = DateTimeOffset.UtcNow;
            }
        }

        [JsonIgnore]
        public bool IsUndefined
        {
            get => Status.ToLower() == "undefined";
        }

        [JsonIgnore]
        public int SelectionOrder
        {
            get
            {
                switch (Status.ToLower())
                {
                    case "unselected":
                        return 0;
                    case "selected":
                        return 1;
                    default:
                        return -1;
                }
            }
        }

        public void MergeStatus(string status)
        {
            if (Status.ToLower() != "selected")
            {
                Status = status;
            }
        }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }

        public HashSet<string> Sources { get; set; }
        public string Segments { get; set; }
    }

    public class SymptomSourceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
