using System;
using System.Collections.Generic;
using System.Linq;

namespace Dx29.Data
{
    public class PatientGroups : List<PatientGroup>
    {
    }

    public class PatientGroup
    {
        public string Name { get; set; }

        public Country Country { get; set; }

        public string LanguageOfName { get; set; }

        public string Url { get; set; }

    }

    public class Country
    {
        public Name  Name { get; set; }
    }

    public class Name
    {
        public string __text { get; set; }
    }



}
