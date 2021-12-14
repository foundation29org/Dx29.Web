using System;
using System.Linq;
using System.Collections.Generic;

using Dx29.Data;

namespace Dx29.Web.Models
{
    public class AnalysisModel
    {
        public string Id { get; set; }

        public IDictionary<string, TermDesc> Symptoms { get; set; }
        public IDictionary<string, TermDesc> Diseases { get; set; }

        public string Status { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }

        static public AnalysisModel CreateFromResource(ResourceAnalysis resource)
        {
            var model = new AnalysisModel
            {
                Id = resource.Id,
                Symptoms = resource.GetSymptoms().ToDictionary(r => r, r => (TermDesc)null),
                Status = resource.Status,
                CreatedOn = resource.CreatedOn,
                UpdatedOn = resource.UpdatedOn
            };
            model.Diseases = model.Status.EqualsNoCase("Ready") ? resource.GetDiseases().ToDictionary(r => r, r => (TermDesc)null) : new Dictionary<string, TermDesc>();
            return model;
        }
    }
}
