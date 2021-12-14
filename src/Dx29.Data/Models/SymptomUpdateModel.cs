using System;
using System.Collections.Generic;

namespace Dx29.Web.Models
{
    public class SymptomsUpdateModel : List<SymptomUpdateModel>
    {
    }

    public class SymptomUpdateModel
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; }
        public HashSet<string> Sources { get; set; }
    }
}
