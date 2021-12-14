using System;
using System.Collections.Generic;
using System.Linq;

namespace Dx29.Data
{
    public class PhenSimilarity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Link => $"https://hpo.jax.org/app/browse/term/{Id}";

        public bool HasPatient { get; set; }
        public bool HasDisease { get; set; }
        public string Relationship { get; set; }
        public double Score { get; set; }

        public string RelatedId { get; set; }
        public string RelatedName { get; set; }
        public string RelatedDesc { get; set; }

        public IList<Reference> Categories { get; set; }
        public Reference FirstCategory => Categories?.FirstOrDefault();

        public HPOTerm Frequency { get; set; } = new HPOTerm
        {
            Id = "HP:9999999",
            Name = null,
            Desc = ""
        };

    }
}
