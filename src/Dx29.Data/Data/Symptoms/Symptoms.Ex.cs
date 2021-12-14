using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Dx29.Data
{
    partial class Symptom
    {
        [JsonIgnore]
        public bool IsKeySymptom
        {
            get => Status.ToLower() == "selected";
            set
            {
                Status = value ? "Selected" : "Preselected";
                LastUpdate = DateTimeOffset.UtcNow;
            }
        }

        [JsonIgnore]
        public bool IsSelected
        {
            get => Status.ToLower() == "selected" || Status.ToLower() == "preselected";
            set
            {
                Status = value ? "Selected" : "Unselected";
                LastUpdate = DateTimeOffset.UtcNow;
            }
        }

        [JsonIgnore]
        public bool IsRemoved
        {
            get => Status.ToLower() == "removed";
            set
            {
                Status = value ? "Removed" : "Unselected";
                LastUpdate = DateTimeOffset.UtcNow;
            }
        }

        [JsonIgnore]
        public bool IsUnselected
        {
            get => Status.ToLower() == "unselected";
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
                return Status.ToLower() switch
                {
                    "unselected" => 0,
                    "selected" => 1,
                    _ => -1,
                };
            }
        }

        [JsonIgnore]
        public bool HasDocument
        {
            get => Sources.Any(r => r.Name != "Manual");
        }

        public void MergeLastUpdate(DateTimeOffset dateTime)
        {
            if (dateTime > LastUpdate)
            {
                LastUpdate = dateTime;
            }
        }

        static public string MergeStatus(string status1, string status2)
        {
            if (status1.ToLower() != "selected")
            {
                return status2;
            }
            return status1;
        }
    }
}
