using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class ExomiserJSON : List<ExomiserGene>
    {
    }

    public class ExomiserGene
    {
        public string GeneSymbol { get; set; }
        public double CombinedScore { get; set; }
        public double VariantScore { get; set; }
        public GeneIdentifier GeneIdentifier { get; set; }
        public List<GeneScores> GeneScores { get; set; }
        public Dictionary<string, PriorityResult> PriorityResults { get; set; }
    }

    public class GeneIdentifier
    {
        public string GeneSymbol { get; set; }
        public string HgncId { get; set; }
    }

    public class GeneScores
    {
        public string ModeOfInheritance { get; set; }
        public double CombinedScore { get; set; }
        public double PhenotypeScore { get; set; }
        public double VariantScore { get; set; }
        public List<ContributingVariants> ContributingVariants { get; set; }
    }

    public class ContributingVariants
    {
        public string ChromosomeName { get; set; }
        public string GenomeAssembly { get; set; }
        public double Position { get; set; }
        public string Ref { get; set; }
        public string Alt { get; set; }
        public string VariantEffect { get; set; }
        public bool WhiteListed { get; set; }
        public FrequencyData FrequencyData { get; set; }
        public PathogenicityData PathogenicityData { get; set; }
        public List<TranscriptAnnotations> TranscriptAnnotations { get; set; }
    }

    public class FrequencyData
    {
        public RsId RsId { get; set; }
        public List<KnownFrequencies> KnownFrequencies { get; set; }
    }

    public class RsId
    {
        public int Id { get; set; }
        public bool Empty { get; set; }
    }

    public class KnownFrequencies
    {
        public string Source { get; set; }
        public double Frequency { get; set; }
    }

    public class PathogenicityData
    {
        public ClinVarData ClinVarData { get; set; }
        public List<PredictedPathogenicityScores> PredictedPathogenicityScores { get; set; }
    }

    public class ClinVarData
    {
        public string AlleleId { get; set; }
        public string PrimaryInterpretation { get; set; }
        public List<string> SecondaryInterpretations { get; set; }
        public string ReviewStatus { get; set; }
    }

    public class PredictedPathogenicityScores
    {
        public string Source { get; set; }
        public double Score { get; set; }
    }

    public class TranscriptAnnotations
    {
        public string GeneSymbol { get; set; }
        public string HgvsProtein { get; set; }
    }

    public class Aminoacid : List<AminoAcidContent>
    {
    }

    public class AminoAcidContent
    {
        public string Term { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class ClinVar : Dictionary<string, ClinVarContent>
    {
    }

    public class ClinVarContent
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Color { get; set; }
    }
    public class Frequency : Dictionary<string, FrequencyContent>
    {
    }

    public class FrequencyContent
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }

    public class Inheritance : Dictionary<string, InheritanceContent>
    {
    }

    public class InheritanceContent
    {
        public string Name { get; set; }
        public string NameShort { get; set; }
        public string Desc { get; set; }
        public string Link { get; set; }
        public string Id { get; set; }
    }

    public class OtherAnnotations : Dictionary<string, OtherAnnotationsContent>
    {
    }

    public class OtherAnnotationsContent
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Color { get; set; }
    }

    public class VariantEffect : Dictionary<string, VariantEffectContent>
    {
    }

    public class VariantEffectContent
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Link { get; set; }
        public string Id { get; set; }
        public string Impact { get; set; }
        public string Color { get; set; }
    }

    public class PriorityResult
    {
        public List<AssociatedDisease> AssociatedDiseases { get; set; }
    }

    public class AssociatedDisease
    {
        public string DiseaseId { get; set; }
    }
}
