using System;
using System.Collections.Generic;
using System.Linq;

namespace Dx29.Data
{
    public class DiffDisease
    {
        public DiffDisease()
        {
            Symptoms = new List<DiffSymptom>();
            Courses = new List<ClinicalInfo>();
            Modifiers = new List<ClinicalInfo>();
            XRefs = new List<string>();
        }

        public string Id { get; set; }
        public string Name { get; set; } 
        public string Desc { get; set; }

        public int ScoreDx29 { get; set; }
        public int ScoreGenes { get; set; }
        public int ScoreSymptoms { get; set; }

        public string Type { get; set; }
        public List<DiffSymptom> Symptoms { get; set; }

        public MatchesGenes Genes { get; set; }
        public List<ClinicalInfo> Courses { get; set; }
        public List<ClinicalInfo> Modifiers { get; set; }
        public List<string> XRefs { get; set; }
        public bool IsSelected { get; set; }

        public bool HasGenes() => Genes?.Count > 0;
    }

    public class HPOTerm
    {
        public string Id { get; set; } 
        public string Name { get; set; } 
        public string Desc { get; set; } 
        public string Link => $"https://hpo.jax.org/app/browse/term/{Id}";
    }

    public class DiffSymptom : HPOTerm
    {
        public HPOTerm Frequency { get; set; }
        public bool HasPatient { get; set; }
        public bool HasDisease { get; set; }
        public string Relationship { get; set; }
        public double Score { get; set; }

        public string RelatedId { get; set; }
        public string RelatedName { get; set; } 
        public string RelatedDesc { get; set; }

        public IList<Reference> Categories { get; set; }
        public Reference FirstCategory => Categories?.FirstOrDefault();

    }

    public class ClinicalInfo : HPOTerm
    {
    }

    public class MatchesGenes: Dictionary <string, GeneInfo>
    {

    }

    public class GeneInfo 
    {
        public string label { get; set; }
    }

    public class XRefs
    {
        public string Reference { get; set; }
        public XRefData Info { get; set; }

    }

    public class XRefData
    {
        public string Id { get; set; }
        public string Link { get; set; }
    }
}
