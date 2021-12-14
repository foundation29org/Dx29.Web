using System;
using System.Linq;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class DataAnalysisInfo
    {
        public DataAnalysisInfo()
        {
            symptoms = new List<string>();
            genes = new List<DataAnalysisGene>();
        }

        public IList<string> symptoms { get; set; }
        public IList<DataAnalysisGene> genes { get; set; }
    }

    public class DataAnalysisGene
    {
        public DataAnalysisGene()
        {
            diseases = new List<string>();
        }

        public string name { get; set; }
        public double score { get; set; }
        public double? combinedScore { get; set; }
        public IList<string> diseases { get; set; }

        public static DataAnalysisGene FromResourceGene(ResourceGene gene)
        {
            return new DataAnalysisGene
            {
                name = gene.Id.ToUpper(),
                score = gene.GetScore(),
                diseases = gene.GetDiseases()
            };
        }

        public static DataAnalysisGene FromExomiser(ExomiserGene gene)
        {
            return new DataAnalysisGene
            {
                name = gene.GeneSymbol,
                score = gene.VariantScore,
                diseases = gene.PriorityResults.Values.SelectMany(r => r.AssociatedDiseases).Select(r => r.DiseaseId).ToArray()
            };
        }
    }
}
