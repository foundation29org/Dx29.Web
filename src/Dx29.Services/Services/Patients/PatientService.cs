using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


using Dx29.Data;
using Dx29.Web.Models;
using Dx29.Services;

namespace Dx29.Web.Services
{
    public partial class PatientService
    {
        public MedicalHistoryClient MedicalHistoryClient { get; }
        public PhenReportsService PhenReportsService { get; }
        public GeneReportsService GeneReportsService { get; }

        private readonly UserManager<ApplicationUser> _userManager;
        public EmailHelper _emailHelper;
        public IHttpContextAccessor ContextAccessor { get; }
        public AccountHashService AccountHashService { get; }


        public PatientService(UserManager<ApplicationUser> userManager, MedicalHistoryClient medicalHistoryClient, PhenReportsService phenReportsService, GeneReportsService geneReportsService, EmailHelper emailHelper, IHttpContextAccessor contextAccessor, AccountHashService accountHashService) 
        {
            MedicalHistoryClient = medicalHistoryClient;
            PhenReportsService = phenReportsService;
            GeneReportsService = geneReportsService;
            _userManager = userManager;
            _emailHelper = emailHelper;
            ContextAccessor = contextAccessor;
            AccountHashService = accountHashService;
        }

        public async Task<IList<PatientModel>> GetPatientsAsync(string userId, bool includeDeleted = false)
        {
            var medicalCases = await MedicalHistoryClient.GetMedicalCasesAsync(userId, includeDeleted);
            if (medicalCases != null)
            {
                return medicalCases.Select(r => r.AsPatientModel()).ToArray();
            }
            return null;
        }

        public async Task<PatientModel> GetPatientAsync(string userId, string caseId)
        {
            var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(userId, caseId);
            if (medicalCase != null)
            {
                return medicalCase.AsPatientModel();
            }
            return null;
        }

        public async Task<PatientModel> CreatePatientAsync(string userId, CreatePatitentModel model)
        {
            Console.WriteLine("CreatePatientAsync Service");
            Console.WriteLine(model.Serialize(true));
            var medicalCase = await MedicalHistoryClient.CreateMedicalCaseAsync(userId, model.PatientInfo.AsPatientInfo());
            if (medicalCase != null)
            {
                // Create default ResourceGroups
                await MedicalHistoryClient.CreateResourceGroupAsync(userId, medicalCase.Id, ResourceGroupType.Reports, "Medical");
                await MedicalHistoryClient.CreateResourceGroupAsync(userId, medicalCase.Id, ResourceGroupType.Reports, "Genetic");
                await MedicalHistoryClient.CreateResourceGroupAsync(userId, medicalCase.Id, ResourceGroupType.Phenotype, "Manual");
                await MedicalHistoryClient.CreateResourceGroupAsync(userId, medicalCase.Id, ResourceGroupType.Genotype, "Manual");
                await MedicalHistoryClient.CreateResourceGroupAsync(userId, medicalCase.Id, ResourceGroupType.Analysis, "Analysis");
                await MedicalHistoryClient.CreateResourceGroupAsync(userId, medicalCase.Id, ResourceGroupType.Notes, "Notes");

                // Add symptoms
                var symptoms = model.Symptoms ?? Array.Empty<string>();
                var resources = symptoms.Select(r => new ResourceSymptom(r, TermStatus.Selected)).ToArray();
                await MedicalHistoryClient.UpsertResourceGroupAsync(userId, medicalCase.Id, ResourceGroupType.Phenotype, "Manual", resources);

                // Add reports
                if (model.Files != null)
                {
                    foreach (var item in model.Files.Where(r => r.IsPhenotype))
                    {
                        await PhenReportsService.ProcessFileAsync(userId, medicalCase.Id, item);
                    }
                    foreach (var item in model.Files.Where(r => r.IsGenotype))
                    {
                        await GeneReportsService.ProcessFileAsync(userId, medicalCase.Id, item);
                    }
                }
                return medicalCase.AsPatientModel();
            }
            return null;
        }

        public async Task<PatientModel> UpdatePatientAsync(string userId, string caseId, PatientInfoModel model)
        {
            var medicalCase = await MedicalHistoryClient.UpdateMedicalCaseAsync(userId, caseId, model.AsPatientInfo());
            if (medicalCase != null)
            {
                return medicalCase.AsPatientModel();
            }
            return null;
        }

        public async Task<MedicalCaseSummary> GetCaseSummaryAsync(string userId, string caseId)
        {
            var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(userId, caseId);
            if (medicalCase != null)
            {
                return medicalCase.AsMedicalCaseSummary();
            }
            return null;
        }

        public async Task DeletePatientAsync(string userId, string caseId, bool force = false)
        {
            await MedicalHistoryClient.DeleteMedicalCaseAsync(userId, caseId, force);
        }

        public async Task DeleteUserPatientsAsync(string userId)
        {
            await MedicalHistoryClient.DeleteUserCasesAsync(userId);
        }
    }
}
