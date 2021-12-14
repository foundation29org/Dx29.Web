using System;
using System.Linq;

namespace Dx29.Data
{
    static public class DiffDiseaseExtensions
    {
        static public (int, int, int, int, int) SymptomsMatch(this DiffDisease disease)
        {
            int patientSymptoms = disease.Symptoms.Where(r => r.HasPatient).Count();
            int diseaseSymptoms = disease.Symptoms.Where(r => r.HasDisease).Count();
            int matchSymptoms = disease.Symptoms.Where(r => r.HasPatient && r.HasDisease).Count();
            int patientPercent = (int)(((double)matchSymptoms / patientSymptoms) * 100);
            int diseasePercent = (int)(((double)matchSymptoms / diseaseSymptoms) * 100);
            return (patientSymptoms, diseaseSymptoms, matchSymptoms, patientPercent, diseasePercent);
        }

        static public (int, int, int) SymptomsVenn(this DiffDisease disease)
        {
            int patientSymptoms = disease.Symptoms.Where(r => r.HasPatient).Count();
            int diseaseSymptoms = disease.Symptoms.Where(r => r.HasDisease).Count();
            int matchSymptoms = disease.Symptoms.Where(r => r.HasPatient && r.HasDisease).Count();
            int totalSymptoms = disease.Symptoms.Count();

            double a = patientSymptoms - matchSymptoms;
            double b = matchSymptoms;
            double c = diseaseSymptoms - matchSymptoms;
            double t = a + b + c;

            int na = (int)Math.Ceiling((a / t) * 100);
            int nb = (int)Math.Ceiling((b / t) * 100);
            int nc = (int)Math.Ceiling((c / t) * 100);

            return (na, nb, nc);
        }

        static public (string, string) GetDiseaseId(this DiffDisease disease)
        {
            try
            {
                var parts = disease.Id.Split(':');
                return (parts[1], parts[0]);
            }
            catch
            {
                return ("", "");
            }
        }

        static public (string, string) GetGenes(this DiffDisease disease)
        {
            if (disease.Genes != null)
            {
                var ids = String.Join(", ", disease.Genes.Select(r => r.Key));
                var labels = String.Join(", ", disease.Genes.Select(r => r.Value.label));
                return (ids, labels);
            }
            return ("", "");
        }
    }
}
