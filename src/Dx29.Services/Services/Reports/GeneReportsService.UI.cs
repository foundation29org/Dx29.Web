using System;
using System.IO;
using System.Collections.Generic;

using Dx29.Data;

namespace Dx29.Web.Services
{
    partial class GeneReportsService
    {
        private IList<GenotypeInfo> DescribeGeneTerms(IList<ExomiserGene> items)
        {
            // Get json resources for translations
            var json_otherAnnotations = File.ReadAllText("Assets/Genotype/otherAnnotations.json");
            var otherAnnotations = json_otherAnnotations.Deserialize<OtherAnnotations>();

            var json_clinVar = File.ReadAllText("Assets/Genotype/clinVar.json");
            var clinVar = json_clinVar.Deserialize<ClinVar>();

            var json_inheritance = File.ReadAllText("Assets/Genotype/inheritance.json");
            var inheritance = json_inheritance.Deserialize<Inheritance>();

            var json_variantEffect = File.ReadAllText("Assets/Genotype/variantEffect.json");
            var variantEffect = json_variantEffect.Deserialize<VariantEffect>();

            var json_aminoacid = File.ReadAllText("Assets/Genotype/amino_acid.json");
            var aminoacid = json_aminoacid.Deserialize<Aminoacid>();

            var json_frequency = File.ReadAllText("Assets/Genotype/frequency.json");
            var frequency = json_frequency.Deserialize<Frequency>();

            // Fill data
            IList<GenotypeInfo> result = new List<GenotypeInfo>();
            foreach (var item in items)
            {
                // Summary
                GenotypeInfo genotypeItem = new GenotypeInfo();

                genotypeItem.Name = item.GeneSymbol;

                var scoreColor = ScoreColors(Math.Round(item.CombinedScore * 100, 2));
                genotypeItem.Score = new Score()
                {
                    Value = item.CombinedScore,
                    Color = scoreColor,
                    TextColor = ScoreTextColors(scoreColor)
                };

                foreach (var infoGen in item.GeneScores)
                {
                    foreach (var contributingVariant in infoGen.ContributingVariants)
                    {
                        if (contributingVariant.WhiteListed)
                        {
                            genotypeItem.Whitelisted = new Whitelisted()
                            {
                                Color = otherAnnotations["whitelist_variant"].Color,
                                Name = otherAnnotations["whitelist_variant"].Name,
                                TextColor = "white"
                            };
                        }
                        if (infoGen.CombinedScore == genotypeItem.Score.Value)
                        {
                            if (contributingVariant.PathogenicityData.ClinVarData.PrimaryInterpretation != "")
                            {
                                var link = "";
                                if (contributingVariant.PathogenicityData.ClinVarData.AlleleId != "")
                                {
                                    link = "https://www.ncbi.nlm.nih.gov/clinvar/?term=" + contributingVariant.PathogenicityData.ClinVarData.AlleleId;
                                }
                                genotypeItem.ClinVar = new List<ClinVarItem>();
                                var clinVarColor = clinVar[contributingVariant.PathogenicityData.ClinVarData.PrimaryInterpretation.ToLower()].Color;
                                genotypeItem.ClinVar.Add(new ClinVarItem()
                                {
                                    Name = clinVar[contributingVariant.PathogenicityData.ClinVarData.PrimaryInterpretation.ToLower()].Name,
                                    Color = clinVarColor,
                                    Link = link,
                                    TextColor = ClinVarTextColor(clinVarColor)
                                });
                            }
                            if (contributingVariant.PathogenicityData.ClinVarData.SecondaryInterpretations.Count > 0)
                            {
                                foreach (var secondaryInterpretation in contributingVariant.PathogenicityData.ClinVarData.SecondaryInterpretations)
                                {
                                    var clinVar2Color = clinVar[secondaryInterpretation.ToLower()].Color;
                                    genotypeItem.ClinVar.Add(new ClinVarItem()
                                    {
                                        Name = clinVar[secondaryInterpretation.ToLower()].Name,
                                        Color = clinVar2Color,
                                        Link = null,
                                        TextColor = ClinVarTextColor(clinVar2Color)
                                    });
                                }
                            }

                        }
                    }
                }

                // Gene Info
                List<GenInfo> genotypeList = new List<GenInfo>();
                foreach (var infoGen in item.GeneScores)
                {
                    GenInfo geneInfo = new GenInfo();
                    foreach (var contributingVariant in infoGen.ContributingVariants)
                    {
                        if (contributingVariant.WhiteListed)
                        {
                            geneInfo.Whitelisted = new Whitelisted()
                            {
                                Color = otherAnnotations["whitelist_variant"].Color,
                                Name = otherAnnotations["whitelist_variant"].Name,
                                TextColor = "white"
                            };
                        }
                        if (contributingVariant.PathogenicityData.ClinVarData.PrimaryInterpretation != "")
                        {
                            var link = "";
                            if (contributingVariant.PathogenicityData.ClinVarData.AlleleId != "")
                            {
                                link = "https://www.ncbi.nlm.nih.gov/clinvar/?term=" + contributingVariant.PathogenicityData.ClinVarData.AlleleId;
                            }
                            geneInfo.ClinVar = new List<ClinVarItem>();
                            var clinVarColor = clinVar[contributingVariant.PathogenicityData.ClinVarData.PrimaryInterpretation.ToLower()].Color;
                            geneInfo.ClinVar.Add(new ClinVarItem()
                            {
                                Name = clinVar[contributingVariant.PathogenicityData.ClinVarData.PrimaryInterpretation.ToLower()].Name,
                                Color = clinVarColor,
                                Link = link,
                                TextColor = ClinVarTextColor(clinVarColor)
                            });
                        }
                        if (contributingVariant.PathogenicityData.ClinVarData.SecondaryInterpretations.Count > 0)
                        {
                            foreach (var secondaryInterpretation in contributingVariant.PathogenicityData.ClinVarData.SecondaryInterpretations)
                            {
                                var clinVarColor2 = clinVar[secondaryInterpretation.ToLower()].Color;
                                geneInfo.ClinVar.Add(new ClinVarItem()
                                {
                                    Name = clinVar[secondaryInterpretation.ToLower()].Name,
                                    Color = clinVarColor2,
                                    Link = null,
                                    TextColor = ClinVarTextColor(clinVarColor2)
                                });
                            }
                        }

                        var exomiserColor = ScoreColors(Math.Round(infoGen.CombinedScore * 100, 2));
                        geneInfo.ExomiserScore = new Score()
                        {
                            Value = infoGen.CombinedScore,
                            Color = exomiserColor,
                            TextColor = ScoreTextColors(exomiserColor)
                        };

                        var phenotypeColor = ScoreColors(Math.Round(infoGen.PhenotypeScore * 100, 2));
                        geneInfo.PhenotypeScore = new Score()
                        {
                            Value = infoGen.PhenotypeScore,
                            Color = phenotypeColor,
                            TextColor = ScoreTextColors(phenotypeColor)
                        };

                        var variantColor = ScoreColors(Math.Round(infoGen.VariantScore * 100, 2));
                        geneInfo.VariantScore = new Score()
                        {
                            Value = infoGen.VariantScore,
                            Color = variantColor,
                            TextColor = ScoreTextColors(variantColor)
                        };

                        geneInfo.ModeOfInheritance = new ModeOfInheritance()
                        {
                            Name = inheritance[infoGen.ModeOfInheritance.ToUpper()].Name,
                            NameShort = inheritance[infoGen.ModeOfInheritance.ToUpper()].NameShort,
                            Link = inheritance[infoGen.ModeOfInheritance.ToUpper()].Link
                        };

                        geneInfo.Mutation = new List<Mutation>();

                        if (contributingVariant.FrequencyData.RsId.Id != 0)
                        {
                            geneInfo.Mutation.Add(new Mutation()
                            {
                                Name = contributingVariant.Ref + " > " + contributingVariant.Alt,
                                Link = "https://www.ncbi.nlm.nih.gov/snp/rs" + contributingVariant.FrequencyData.RsId.Id
                            });
                        }
                        else
                        {
                            geneInfo.Mutation.Add(new Mutation()
                            {
                                Name = contributingVariant.Ref + " > " + contributingVariant.Alt,
                                Link = null
                            });
                        }

                        geneInfo.Chromosome = "chr" + contributingVariant.ChromosomeName + ":" + contributingVariant.Position + " (" + contributingVariant.GenomeAssembly + ")";
                        geneInfo.VariantEffect = new VariantEffect_()
                        {
                            Name = variantEffect[contributingVariant.VariantEffect.ToUpper()].Name,
                            Color = variantEffect[contributingVariant.VariantEffect.ToUpper()].Color,
                            Link = variantEffect[contributingVariant.VariantEffect.ToUpper()].Link,
                            TextColor = VariantEffectTextColor(contributingVariant.VariantEffect.ToUpper())
                        };

                        geneInfo.Literature = new List<LiteratureItem>();
                        foreach (var transcriptAnnotation in contributingVariant.TranscriptAnnotations)
                        {
                            if (transcriptAnnotation.HgvsProtein != "")
                            {
                                if (!transcriptAnnotation.HgvsProtein.Contains("p.(=)") && !transcriptAnnotation.HgvsProtein.Contains("p.?") && !transcriptAnnotation.HgvsProtein.Contains("p.0?"))
                                {
                                    var transCriptAnnotationsProtein1 = transcriptAnnotation.HgvsProtein.Split("p.(");
                                    var transCriptAnnotationsProtein2 = transCriptAnnotationsProtein1[1].Split(")");
                                    var transCriptAnnotationsProtein3 = transCriptAnnotationsProtein2[0];
                                    bool foundInAminoacid = false;
                                    for (var l = 0; l < aminoacid.Count; l++)
                                    {
                                        if (transCriptAnnotationsProtein3.IndexOf(aminoacid[l].Code) > -1)
                                        {
                                            transCriptAnnotationsProtein3 = transCriptAnnotationsProtein3.Replace(aminoacid[l].Code, aminoacid[l].Term);
                                            foundInAminoacid = true;
                                        }
                                    }
                                    if (foundInAminoacid)
                                    {
                                        geneInfo.Literature.Add(new LiteratureItem()
                                        {
                                            Name = "GENE: " + transcriptAnnotation.GeneSymbol + ", VARIANT: " + transCriptAnnotationsProtein3,
                                            Link = "https://mastermind.genomenon.com/detail?gene=" + transcriptAnnotation.GeneSymbol + "&mutation=" + transCriptAnnotationsProtein3
                                        });
                                    }

                                }
                            }
                        }
                        geneInfo.Literature.Add(new LiteratureItem()
                        {
                            Name = "GENE: " + item.GeneSymbol,
                            Link = "https://mastermind.genomenon.com/detail?gene=" + item.GeneSymbol
                        });

                        geneInfo.Frequency = new List<FrequencyItem>();
                        foreach (var frequencyItem in contributingVariant.FrequencyData.KnownFrequencies)
                        {
                            geneInfo.Frequency.Add(new FrequencyItem()
                            {
                                Name = frequency[frequencyItem.Source].Name,
                                Value = frequencyItem.Frequency,
                                Link = frequency[frequencyItem.Source].Link
                            });
                        }

                        geneInfo.PredictedPathogenicityScores = new List<PredictedPathogenicityScores_>();
                        foreach (var predictedPathogenicityScores in contributingVariant.PathogenicityData.PredictedPathogenicityScores)
                        {
                            geneInfo.PredictedPathogenicityScores.Add(new PredictedPathogenicityScores_()
                            {
                                Name = predictedPathogenicityScores.Source,
                                Value = predictedPathogenicityScores.Score
                            });
                        }

                    }
                    if (infoGen.ContributingVariants.Count > 0)
                    {
                        genotypeList.Add(geneInfo);
                    }

                }
                genotypeItem.GenInfo = genotypeList;
                result.Add(genotypeItem);
            }
            return result;
        }

        private string ScoreColors(double scoreValue)
        {
            List<int> color1 = new List<int> { 121, 246, 89 };//89
            List<int> color2 = new List<int> { 87, 96, 211 };
            var w1 = (scoreValue / 100);
            var w2 = 1 - w1;
            List<double> rgb = new List<double>{Math.Round(color1[0] * w1 + color2[0] * w2),
                Math.Round(color1[1] * w1 + color2[1] * w2),
                Math.Round(color1[2] * w1 + color2[2] * w2) };

            var c1 = Convert.ToString((Int16)rgb[0], 16);
            if (c1.Length == 1)
            {
                c1 = "0" + 0;
            }
            var c2 = Convert.ToString((Int16)rgb[1], 16);

            if (c2.Length == 1)
            {
                c2 = "0" + 0;
            }
            var c3 = Convert.ToString((Int16)rgb[2], 16);
            if (c3.Length == 1)
            {
                c3 = "0" + 0;
            }

            return "#" + c1 + c2 + c3;
        }

        private string ScoreTextColors(string scoreColor)
        {
            System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml(scoreColor);
            var colorCode = (col.R * 0.2126 + col.G * 0.7152 + col.B * 0.0722 > 255 / 2) ? "#000000" : "#FFFFFF";
            return colorCode;
        }

        private string ClinVarTextColor(string ClinVarColor)
        {
            if (ClinVarColor != "#A8ACB1" && ClinVarColor != "white")
            {
                return "white";
            }
            else
            {
                return "black";
            }
        }

        private string VariantEffectTextColor(string variantEffectKey)
        {
            string fontColor = "black";

            var json_variantEffect = File.ReadAllText("Assets/Genotype/variantEffect.json");
            var variantEffect = json_variantEffect.Deserialize<VariantEffect>();

            string priority = variantEffect[variantEffectKey].Impact;
            switch (priority)
            {
                case "High":
                    fontColor = "white";
                    break;
                case "Moderate":
                    fontColor = "white";
                    break;
                case "Low":
                    fontColor = "white";
                    break;
                case "Modifier":
                    fontColor = "white";
                    break;
            };

            return fontColor;
        }
    }
}
