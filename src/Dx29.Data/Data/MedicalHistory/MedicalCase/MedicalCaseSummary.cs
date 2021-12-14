using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class MedicalCaseSummary
    {
        public MedicalCaseSummary()
        {
            Symptoms = new SymptomsSummary();
            Genes = new GenesSummary();
            Reports = new ReportsSummary();
            Genotype = new GenotypeSummary();
            DataAnalysis = new DataAnalysisSummary();

            LastUpdate = DateTimeOffset.MinValue;
        }

        public SymptomsSummary Symptoms { get; set; }
        public GenesSummary Genes { get; set; }
        public ReportsSummary Reports { get; set; }
        public GenotypeSummary Genotype { get; set; }
        public DataAnalysisSummary DataAnalysis { get; set; }

        public DateTimeOffset LastUpdate { get; set; }
    }

    public class SymptomsSummary
    {
        public SymptomsSummary()
        {
            Status = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            LastUpdate = DateTimeOffset.MinValue;
        }

        public IDictionary<string, string> Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }

    public class GenesSummary
    {
        public GenesSummary()
        {
            Status = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            LastUpdate = DateTimeOffset.MinValue;
        }

        public IDictionary<string, string> Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }

    public class ReportsSummary
    {
        public ReportsSummary()
        {
            Status = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            LastUpdate = DateTimeOffset.MinValue;
        }

        public IDictionary<string, string> Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }

    public class GenotypeSummary
    {
        public GenotypeSummary()
        {
            Status = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            LastUpdate = DateTimeOffset.MinValue;
        }

        public IDictionary<string, string> Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }

    public class DataAnalysisSummary
    {
        public DataAnalysisSummary()
        {
            Status = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            LastUpdate = DateTimeOffset.MinValue;
        }

        public IDictionary<string, string> Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }
}
