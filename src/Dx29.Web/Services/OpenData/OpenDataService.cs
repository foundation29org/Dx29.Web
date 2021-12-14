using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Dx29.Data;
using Dx29.Services;
using Dx29.Web.Models;

namespace Dx29.Web.Services
{
    public class OpenDataService 
    {
        public OpenDataService(PatientService patientService, TimeLineService timeLineService, DataAnalysisService dataAnalysisService, AccountHashService accountHashService, IConfiguration configuration)
        {
            PatientService = patientService;
            TimeLineService = timeLineService;
            DataAnalysisService = dataAnalysisService;
            AccountHashService = accountHashService;
            BlobStorage = new BlobStorage(configuration.GetConnectionString("OpenDataBlobStorage"));
        }

        public PatientService PatientService { get; }
        public TimeLineService TimeLineService { get; }
        public DataAnalysisService DataAnalysisService { get; }
        public AccountHashService AccountHashService { get; }
        public BlobStorage BlobStorage { get; }

        public async Task CreateOpenDataCaseAsync(string email, string token)
        {
            try
            {
                var name = email.Split('@')[0];

                var openData = await GetOpenDataSymptomsAsync(token);
                if (openData == null)
                {
                    openData = new OpenData();
                }
                var symptoms = openData.Symptoms.Select(r => r.Id).ToArray();
                var model = new CreatePatitentModel(name, PatientGender.Unknown, null)
                {
                    Symptoms = symptoms
                };
                string userId = AccountHashService.GetHash(email);
                var medicalCase = await PatientService.CreatePatientAsync(userId, model);
                if (medicalCase != null)
                {
                    await TimeLineService.UpsertTimeLineAsync(userId, medicalCase.Id, openData.AsTimeLine());
                    await DataAnalysisService.CreateAnalysisAsync(userId, medicalCase.Id, symptoms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task<OpenData> GetOpenDataSymptomsAsync(string token)
        {
            try
            {
                var path = InferPath(token);
                return await BlobStorage.DownloadObjectAsync<OpenData>("open-data", path);
            }
            catch
            {
                return null;
            }
        }

        static private string InferPath(string token)
        {
            var yy = token.Substring(0, 2);
            var mm = token.Substring(2, 2);
            var dd = token.Substring(4, 2);
            return $"{yy}/{mm}/{dd}/{token}/info.json";
        }
    }

    public class OpenData
    {
        public OpenData()
        {
            Symptoms = new List<OpenDataSymptom>();
        }

        public IList<OpenDataSymptom> Symptoms { get; set; }

        public SymptomTimeline AsTimeLine()
        {
            var timeline = new SymptomTimeline();
            foreach (var symptom in Symptoms)
            {
                if (symptom.StartDate != null)
                {
                    var item = new SymptomTimelineItem
                    {
                        Id = symptom.Id,
                        StartDate = symptom.StartDate,
                        EndDate = symptom.EndDate,
                        IsCurrent = symptom.IsCurrent,
                        Notes = symptom.Notes
                    };
                    timeline.Items.Add(item);
                }
            }
            return timeline;
        }
    }

    public class OpenDataSymptom
    {
        public string Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string Notes { get; set; }
    }
}
