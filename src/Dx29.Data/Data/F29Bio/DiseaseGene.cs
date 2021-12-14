using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class GeneDesc
    {
        public string Id { get; set; }
        public string Label { get; set; }
    }

    public class DiseaseGene:Dictionary<string,TermDiseaseGene>
    {
       
    }

    public class TermDiseaseGene
    {
        public string Id { get; set; }
        public string label { get; set; }
        public Dictionary<string, DiseaseGeneContent> genes {get;set;}
    }

    public class DiseaseGeneContent
    {
        public string Id { get; set; }
        public string label { get; set; }
    }
}
