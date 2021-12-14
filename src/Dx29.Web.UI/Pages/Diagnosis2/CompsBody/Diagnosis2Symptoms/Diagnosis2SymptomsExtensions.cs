using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dx29.Data
{
    static public class Diagnosis2SymptomsExtensions
    {
        static public (int, int, int, int, int) Diagnosis2SymptomsMatch(this List<PhenSimilarity> disease)
        {
            int patientSymptoms = disease.Where(r => r.HasPatient).Count();
            int diseaseSymptoms = disease.Where(r => r.HasDisease).Count();
            int matchSymptoms = disease.Where(r => r.HasPatient && r.HasDisease).Count();
            int patientPercent = (int)(((double)matchSymptoms / patientSymptoms) * 100);
            int diseasePercent = (int)(((double)matchSymptoms / diseaseSymptoms) * 100);
            return (patientSymptoms, diseaseSymptoms, matchSymptoms, patientPercent, diseasePercent);
        }

        static public (int, int, int) Diagnosis2SymptomsVenn(this List<PhenSimilarity> disease)
        {
            int patientSymptoms = disease.Where(r => r.HasPatient).Count();
            int diseaseSymptoms = disease.Where(r => r.HasDisease).Count();
            int matchSymptoms = disease.Where(r => r.HasPatient && r.HasDisease).Count();
            int totalSymptoms = disease.Count();

            double a = patientSymptoms - matchSymptoms;
            double b = matchSymptoms;
            double c = diseaseSymptoms - matchSymptoms;
            double t = a + b + c;

            int na = (int)Math.Ceiling((a / t) * 100);
            int nb = (int)Math.Ceiling((b / t) * 100);
            int nc = (int)Math.Ceiling((c / t) * 100);

            return (na, nb, nc);
        }
    }
}
