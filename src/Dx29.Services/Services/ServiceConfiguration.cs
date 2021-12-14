using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Dx29.Web.Services;

namespace Dx29.Services
{
    static public class ServiceConfiguration
    {

        static public void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddManagement(configuration);
            services.AddBioEntity(configuration);
            services.AddF29BioEntity(configuration);
            services.AddLocalization(configuration);
            services.AddPhenSimilarity(configuration);
            services.AddDataAnalysis(configuration);
            services.AddTermSearch(configuration);
            services.AddFileStorage(configuration);
            services.AddFileStorage2(configuration);
            services.AddMedicalHistory(configuration);
            services.AddAnnotations(configuration);
            services.AddAccountHash(configuration);
            services.AddExomiser(configuration);
            services.AddSignalR(configuration);
            services.AddDocuments(configuration);
            services.AddMailing(configuration);
            services.AddLegacy(configuration);
            services.AddOpenDx29(configuration);

            services.AddSingleton<EmailHelper>();
            services.AddTransient<PatientService>();
            services.AddSingleton<SymptomsService>();
            services.AddSingleton<PhenReportsService>();
            services.AddSingleton<GeneReportsService>();
            services.AddSingleton<NotesService>();
            services.AddSingleton<TimeLineService>();
            services.AddSingleton<DiagnosisService>();


        }

        static public void AddManagement(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ManagementClient>();
            services.AddHttpClient<ManagementClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["Management:Endpoint"]);
            });
        }

        static public void AddBioEntity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<BioEntityClient>();
            services.AddHttpClient<BioEntityClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["BioEntity:Endpoint"]);
            });
        }
        static public void AddF29BioEntity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<F29BioEntityClient>();
            services.AddHttpClient<F29BioEntityClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["Diagnosis:Endpoint"]);
            });
        }

        static public void AddLocalization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<LocalizationClient>();
            services.AddHttpClient<LocalizationClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["Localization:Endpoint"]);
            });
        }

        static public void AddPhenSimilarity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<PhenSimilarityClient>();
            services.AddHttpClient<PhenSimilarityClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["PhenSimilarity:Endpoint"]);
            });
        }

        static public void AddDataAnalysis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DataAnalysisService>();
            services.AddHttpClient<DataAnalysisService>(http =>
            {
                http.BaseAddress = new Uri(configuration["Diagnosis:Endpoint"]);
            });
        }

        static public void AddTermSearch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<TermSearchClient>();
            services.AddHttpClient<TermSearchClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["TermSearch:Endpoint"]);
            });
        }

        static public void AddFileStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<FileStorageClient>();
            services.AddHttpClient<FileStorageClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["FileStorage:Endpoint"]);
            });
            services.AddSingleton<FileStorageService>();
        }

        static public void AddFileStorage2(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<FileStorageClient2>();
            services.AddHttpClient<FileStorageClient2>(http =>
            {
                http.BaseAddress = new Uri(configuration["FileStorage:Endpoint"]);
            });
            services.AddSingleton<FileStorageService2>();
        }

        static public void AddMedicalHistory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<MedicalHistoryClient>();
            services.AddHttpClient<MedicalHistoryClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["MedicalHistory:Endpoint"]);
            });
        }

        static public void AddAnnotations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<AnnotationsClient>();
            services.AddHttpClient<AnnotationsClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["Annotations:Endpoint"]);
            });
        }

        static public void AddAccountHash(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton((sp) =>
            {
                return new AccountHashService(configuration["Account:Key"], configuration.GetValue<int>("Account:Inx"), 28);
            });
        }

        static public void AddExomiser(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ExomiserClient>();
            services.AddHttpClient<ExomiserClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["Exomiser:Endpoint"]);
            });
        }

        static public void AddSignalR(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new SignalRService(configuration["SignalR:ConnectionString"], configuration["SignalR:HubName"]));
        }

        static public void AddDocuments(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DocumentsService>();
            services.AddHttpClient<DocumentsService>(http =>
            {
                http.BaseAddress = new Uri(configuration["Documents:Endpoint"]);
            });
        }

        static public void AddMailing(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<MailingService>();
            services.AddHttpClient<MailingService>(http =>
            {
                http.BaseAddress = new Uri(configuration["Mailing:Endpoint"]);
            });
        }

        static public void AddLegacy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<LegacyClient>();
            services.AddHttpClient<LegacyClient>(http =>
            {
                http.BaseAddress = new Uri(configuration["Legacy:Endpoint"]);
            });
        }

        static public void AddOpenDx29(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<OpenDx29Client>();
            services.AddHttpClient<OpenDx29Client>(http =>
            {
                http.BaseAddress = new Uri(configuration["OpenDx29:Endpoint"]);
            });
        }
    }
}
