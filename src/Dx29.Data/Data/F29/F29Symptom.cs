using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class F29Symptom
    {
        public string id { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string comment { get; set; }
        public string frequency { get; set; }
        public IList<F29Synonym> synonyms { get; set; }

        static public F29Symptom FromSymptom(Term term)
        {
            var cls = new F29Symptom
            {
                id = term.Id,
                name = term.Name,
                desc = term.Desc,
                comment = term.Comment,
                frequency = term.Frequency,
                synonyms = new List<F29Synonym>()
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

    public class F29Synonym
    {
        public string label { get; set; }
        public string scope { get; set; }
        public string type { get; set; }
        public string xrefs { get; set; }

        static public F29Synonym FromSynonym(Synonym source)
        {
            return new F29Synonym()
            {
                label = source.Label,
                scope = source.Scope,
                type = source.Type,
                xrefs = source.XRefs,
            };
        }
    }
}
