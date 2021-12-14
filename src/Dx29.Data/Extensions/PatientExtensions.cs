using System;
using System.Linq;
using System.Collections.Generic;

using Dx29.Data;
using Dx29.Web.Models;

namespace Dx29.Web.Services
{
    static public class PatientExtensions
    {
        static public PatientModel AsPatientModel(this MedicalCase source)
        {
            var model = new PatientModel()
            {
                Id = source.Id,
                Status = source.Status,
                PatientInfo = CreatePatientInfoModel(source.PatientInfo),
                SymptomsCount = GetResourceCount(source.ResourceGroups.Values, ResourceGroupType.Phenotype),
                PhenotypeReportsCount = GetResourceCount(source.ResourceGroups.Values, ResourceGroupType.Reports, "Medical"),
                GenotypeReportsCount = GetResourceCount(source.ResourceGroups.Values, ResourceGroupType.Reports, "Genetic"),
                SharedWith = source.SharedWith,
                CreatedOn = source.CreatedOn,
                UpdatedOn = source.UpdatedOn,
            };
            return model;
        }

        static public PatientInfo AsPatientInfo(this PatientInfoModel source)
        {
            return new PatientInfo()
            {
                Name = source.Name,
                Gender = source.Gender,
                BirthDate = source.BirthDate,
                DiseasesIds = source.DiseasesIds ?? new List<string>()
            };
        }

        static public MedicalCaseSummary AsMedicalCaseSummary(this MedicalCase source)
        {
            var model = new MedicalCaseSummary();

            // TODO: Prioritize 'Manual' group. Merge status for future 'deleted' status
            foreach (var resourceGroupRef in source.ResourceGroups.Values.Where(r => r.Type == ResourceGroupType.Phenotype.ToString()))
            {
                var statusDic = model.Symptoms.Status;
                foreach (var resource in resourceGroupRef.Resources)
                {
                    if (!statusDic.ContainsKey(resource.Key) || !statusDic[resource.Key].EqualsNoCase("Selected"))
                    {
                        statusDic[resource.Key] = resource.Value;
                    }
                }
                model.Symptoms.LastUpdate = MaxDate(model.Symptoms.LastUpdate, resourceGroupRef.LastUpdate);
            }
            model.LastUpdate = model.Symptoms.LastUpdate;

            foreach (var resourceGroupRef in source.ResourceGroups.Values.Where(r => r.Type == ResourceGroupType.Genotype.ToString()))
            {
                var statusDic = model.Genes.Status;
                foreach (var resource in resourceGroupRef.Resources)
                {
                    if (!statusDic.ContainsKey(resource.Key) || !statusDic[resource.Key].EqualsNoCase("Selected"))
                    {
                        statusDic[resource.Key] = resource.Value;
                    }
                }
                model.Genes.LastUpdate = MaxDate(model.Genes.LastUpdate, resourceGroupRef.LastUpdate);
            }
            model.LastUpdate = MaxDate(model.LastUpdate, model.Genes.LastUpdate);

            foreach (var resourceGroupRef in source.ResourceGroups.Values.Where(r => r.Type == ResourceGroupType.Reports.ToString() && r.Name == "Medical"))
            {
                var statusDic = model.Reports.Status;
                foreach (var resource in resourceGroupRef.Resources)
                {
                    statusDic[resource.Key] = resource.Value;
                }
                model.Reports.LastUpdate = MaxDate(model.Reports.LastUpdate, resourceGroupRef.LastUpdate);
            }
            model.LastUpdate = MaxDate(model.LastUpdate, model.Reports.LastUpdate);

            foreach (var resourceGroupRef in source.ResourceGroups.Values.Where(r => r.Type == ResourceGroupType.Reports.ToString() && r.Name == "Genetic"))
            {
                var statusDic = model.Genotype.Status;
                foreach (var resource in resourceGroupRef.Resources)
                {
                    statusDic[resource.Key] = resource.Value;
                }
                model.Genotype.LastUpdate = MaxDate(model.Genotype.LastUpdate, resourceGroupRef.LastUpdate);
            }
            model.LastUpdate = MaxDate(model.LastUpdate, model.Genotype.LastUpdate);

            foreach (var resourceGroupRef in source.ResourceGroups.Values.Where(r => r.Type == ResourceGroupType.Analysis.ToString()))
            {
                var analysisDic = model.DataAnalysis.Status;
                foreach (var resource in resourceGroupRef.Resources)
                {
                    analysisDic[resource.Key] = resource.Value;
                }
                model.DataAnalysis.LastUpdate = MaxDate(model.DataAnalysis.LastUpdate, resourceGroupRef.LastUpdate);
            }

            return model;
        }

        #region Helpers
        private static int GetResourceCount(IEnumerable<ResourceGroupRef> resourceGroups, ResourceGroupType type)
        {
            return resourceGroups.Where(r => r.Type == type.ToString())
                .SelectMany(r => r.Resources)
                .Where(r => IsAccountable(r.Value))
                .Select(r => r.Key).Distinct().Count();
        }
        private static int GetResourceCount(IEnumerable<ResourceGroupRef> resourceGroups, ResourceGroupType type, string name)
        {
            return resourceGroups.Where(r => r.Type == type.ToString() && r.Name.EqualsNoCase(name))
                .SelectMany(r => r.Resources)
                .Where(r => IsAccountable(r.Value))
                .Select(r => r.Key).Distinct().Count();
        }

        private static bool IsAccountable(string value) => value.EqualsNoCase("Selected") || value.EqualsNoCase("Ready");

        private static PatientInfoModel CreatePatientInfoModel(PatientInfo source)
        {
            return new PatientInfoModel()
            {
                Name = source.Name,
                Gender = source.Gender,
                BirthDate = source.BirthDate,
                DiseasesIds = source.DiseasesIds ?? new List<string>()
            };
        }

        static private DateTimeOffset MaxDate(DateTimeOffset date1, DateTimeOffset date2) => date1 > date2 ? date1 : date2;
        #endregion
    }
}
