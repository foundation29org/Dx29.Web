using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class F29Disease
    {
        public F29Disease()
        {
            phenotypes = new Dictionary<string, F29Symptom>();
            synonyms = new List<F29Synonym>();
            xrefs = new List<string>();
        }

        public string id { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string comment { get; set; }
        public IDictionary<string, F29Symptom> phenotypes { get; set; }
        public IList<F29Synonym> synonyms { get; set; }
        public IList<string> xrefs { get; set; }

        static public F29Disease FromDisease(Term term)
        {
            var cls = new F29Disease
            {
                id = term.Id,
                name = term.Name,
                desc = term.Desc,
                comment = term.Comment,
                xrefs = term.XRefs
            };
            if (term.Synonyms != null)
            {
                foreach (var item in term.Synonyms)
                {
                    cls.synonyms.Add(F29Synonym.FromSynonym(item));
                }
            }
            return cls;
        }
    }
}
