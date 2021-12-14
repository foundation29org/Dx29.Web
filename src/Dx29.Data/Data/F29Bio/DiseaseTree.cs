using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class DiseaseTreeDesc
    {
        public string Id { get; set; }
        public IDictionary<string, SymptomTree> Phenotypes { get; set; }
    }

    public class SymptomTree
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

    }
}
