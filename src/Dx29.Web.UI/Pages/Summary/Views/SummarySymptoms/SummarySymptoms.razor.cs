using System;
using System.Linq;
using System.Collections.Generic;

using Dx29.Data;

namespace Dx29.Web.UI.Components
{
    partial class SummarySymptoms
    {
        private IList<SymptomDesc> GetKeySymptoms()
        {
            return Items.Where(r => r.Status.EqualsNoCase("Selected") || r.Status.EqualsNoCase("Preselected"))
                .OrderBy(r => r.Name)
                .OrderBy(r => r.IsKeySymptom ? 0 : 1)
                .ToArray();
        }

        private IList<SymptomDesc> GetReportSymptoms(string id)
        {
            return Items.Where(r => r.Sources.Any(s => s.Name == id)).ToArray();
        }
    }
}
