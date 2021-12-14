using System;
using System.Linq;
using System.Collections.Generic;

namespace Dx29.Data
{
    static public class CaseSummaryExtensions
    {
        static public bool IsReadyForDiagnosis(this MedicalCaseSummary summary)
        {
            return SelectedSymptoms(summary) > 0 || SelectedGenes(summary) > 0 || IsGenotypeBusy(summary) || IsReportsBusy(summary);
        }

        static public bool IsBusy(this MedicalCaseSummary summary)
        {
            return summary.IsGenotypeBusy() || summary.IsReportsBusy() || summary.IsDataAnalysisBusy();
        }

        //
        //  Symptoms
        //
        static public IList<string> GetEligibleSymptoms(this MedicalCaseSummary summary)
        {
            return summary.Symptoms.Status.Where(r => r.Value.EqualsNoCase("Selected") || r.Value.EqualsNoCase("Preselected")).Select(r => r.Key).ToArray();
        }

        static public int TotalSymptoms(this MedicalCaseSummary summary)
        {
            return summary.Symptoms.Status.Count();
        }

        static public int SelectedSymptoms(this MedicalCaseSummary summary)
        {
            return summary.Symptoms.Status.Count(r => r.Value.EqualsNoCase("Selected"));
        }

        //
        //  Genes
        //
        static public IList<string> GetSelectedGenes(this MedicalCaseSummary summary)
        {
            return summary.Genes.Status.Where(r => r.Value.EqualsNoCase("Selected")).Select(r => r.Key).ToArray();
        }

        static public int TotalGenes(this MedicalCaseSummary summary)
        {
            return summary.Genes.Status.Count();
        }

        static public int SelectedGenes(this MedicalCaseSummary summary)
        {
            return summary.Genes.Status.Count(r => r.Value.EqualsNoCase("Selected"));
        }

        //
        //  Reports
        //
        static public bool IsReportsBusy(this MedicalCaseSummary summary)
        {
            return summary.Reports.Status.Any(r => !IsStatusFinish(r.Value));
        }

        static public int TotalReports(this MedicalCaseSummary summary)
        {
            return summary.Reports.Status.Count();
        }

        static public int ReadyReports(this MedicalCaseSummary summary)
        {
            return summary.Reports.Status.Count(r => r.Value.EqualsNoCase("Ready"));
        }

        static public string ReportsCountDesc(this MedicalCaseSummary summary)
        {
            int ready = summary.ReadyReports();
            int total = summary.TotalReports();
            if (ready == total) return ready.ToString();
            return $"{ready} / {total}";
        }


        //
        //  Genotype
        //
        static public bool IsGenotypeBusy(this MedicalCaseSummary summary)
        {
            return summary.Genotype.Status.Any(r => !IsStatusFinish(r.Value));
        }

        static public int TotalGenotype(this MedicalCaseSummary summary)
        {
            return summary.Genotype.Status.Count();
        }

        static public int ReadyGenotype(this MedicalCaseSummary summary)
        {
            return summary.Genotype.Status.Count(r => r.Value.EqualsNoCase("Ready"));
        }

        static public string GenotypeCountDesc(this MedicalCaseSummary summary)
        {
            int ready = summary.ReadyGenotype();
            int total = summary.TotalGenotype();
            if (ready == total) return ready.ToString();
            return $"{ready} / {total}";
        }

        //
        //  Helpers
        //
        static public bool IsDataAnalysisEmpty(this MedicalCaseSummary summary)
        {
            return summary.DataAnalysis.Status.Count == 0;
        }

        static public bool IsDataAnalysisBusy(this MedicalCaseSummary summary)
        {
            var last = summary.DataAnalysis.Status.OrderBy(r => r.Key).LastOrDefault();
            if (last.Value == null)
            {
                return false;
            }
            if (IsStatusFinish(last.Value))
            {
                return false;
            }
            // Ensure is busy for less that 2 min
            return (DateTime.UtcNow - summary.DataAnalysis.LastUpdate).TotalMinutes < 2;
        }

        static public bool IsDataAnalysisUpToDate(this MedicalCaseSummary summary)
        {
            if (summary.DataAnalysis.LastUpdate < summary.LastUpdate)
            {
                return false;
            }
            var last = summary.DataAnalysis.Status.OrderBy(r => r.Key).LastOrDefault();
            if (last.Value == null)
            {
                return false;
            }
            if (IsStatusFinish(last.Value))
            {
                return true;
            }
            // Ensure is busy for less that 2 min
            return (DateTime.UtcNow - summary.DataAnalysis.LastUpdate).TotalMinutes < 2;
        }

        static public bool IsStatusFinish(string status)
        {
            switch (status?.ToLower())
            {
                case "ready":
                case "canceled":
                case "failed":
                case "error":
                    return true;
                default:
                    return false;
            }
        }
    }
}
