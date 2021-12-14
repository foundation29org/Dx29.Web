using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class GenotypeInfo
    {
        public string Name { get; set; }
        public Whitelisted Whitelisted { get; set; }
        public List<ClinVarItem> ClinVar { get; set; }
        public Score Score { get; set; }
        public List<GenInfo> GenInfo { get; set; }
    }

    public class Whitelisted
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
    }

    public class ClinVarItem
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public string Link { get; set; }
    }

    public class Score
    {
        public double Value { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
    }
    public class GenInfo
    {
        public Whitelisted Whitelisted { get; set; }
        public List<ClinVarItem> ClinVar { get; set; }
        public ModeOfInheritance ModeOfInheritance { get; set; }
        public List<Mutation> Mutation { get; set; }
        public string Chromosome { get; set; }
        public VariantEffect_ VariantEffect { get; set; }
        public Score ExomiserScore { get; set; }
        public Score PhenotypeScore { get; set; }
        public Score VariantScore { get; set; }
        public List<LiteratureItem> Literature { get; set; }
        public List<FrequencyItem> Frequency { get; set; }
        public List<PredictedPathogenicityScores_> PredictedPathogenicityScores { get; set; }

    }

    public class ModeOfInheritance
    {
        public string Name { get; set; }
        public string NameShort { get; set; }

        public string Link { get; set; }
    }

    public class VariantEffect_
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public string Link { get; set; }
    }
    public class Mutation
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }
    public class LiteratureItem
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }

    public class FrequencyItem
    {
        public string Name { get; set; }
        public double Value { get; set; }

        public string Link { get; set; }
    }

    public class PredictedPathogenicityScores_
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

}
