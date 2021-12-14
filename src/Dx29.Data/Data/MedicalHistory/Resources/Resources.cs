using System;
using System.Linq;
using System.Collections.Generic;

namespace Dx29.Data
{
    public enum TermStatus
    {
        Undefined,
        Selected,
        Unselected
    }

    public class ResourceSymptom : Resource
    {
        public ResourceSymptom() { }
        public ResourceSymptom(string hpo, TermStatus status = TermStatus.Undefined) : base(hpo.ToLower(), hpo)
        {
            Status = status.ToString();
        }
    }

    public class ResourceGene : Resource
    {
        public ResourceGene() { }
        public ResourceGene(string id, string name, double score, IList<string> diseases) : base(id.ToLower(), name)
        {
            var status = score >= 0.5 ? TermStatus.Selected : TermStatus.Unselected;
            Status = status.ToString();
            Properties["Score"] = score.ToString("0.00");
            Properties["Diseases"] = String.Join(';', diseases).ToLower();
        }

        public double GetScore() => Double.Parse(Properties["Score"], System.Globalization.CultureInfo.InvariantCulture);

        public IList<string> GetDiseases() => Properties["Diseases"].Split(';').ToArray();
    }

    public class ResourceReport : Resource
    {
        public ResourceReport() { }
        public ResourceReport(string id, string name, long size) : base(id, name)
        {
            Properties["Size"] = size.ToString();
        }
    }

    public class ResourceAnalysis : Resource
    {
        public ResourceAnalysis() { }
        public ResourceAnalysis(string id) : base(id, id)
        {
            Status = "Pending";
        }

        public void SetData(IList<string> symptoms, IList<string> genes, IList<string> genotypeIds, IList<string> diseases)
        {
            Properties["Symptoms"] = String.Join(';', symptoms).ToLower();
            Properties["Genes"] = String.Join(';', genes).ToLower();
            Properties["GenotypeIds"] = String.Join(';', genotypeIds).ToLower();
            Properties["Diseases"] = String.Join(';', diseases).ToLower();
        }

        public IList<string> GetSymptoms() => Properties["Symptoms"].Split(';');
        public IList<string> GetGenes() => Properties["Genes"].Split(';');
        public IList<string> GetGenotypeIds() => Properties["GenotypeIds"].Split(';');
        public IList<string> GetDiseases() => Properties["Diseases"].Split(';');
    }

    public class ResourceDiagnosis : Resource
    {
        public ResourceDiagnosis() { }
        public ResourceDiagnosis(string id, string name) : base(id, name)
        {
        }
    }

    public class ResourceNote : Resource
    {
        public ResourceNote() { }
        public ResourceNote(string id, string name) : base(id, name)
        {
        }
    }
}
