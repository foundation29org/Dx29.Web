using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using Dx29.Data;
using Dx29.Services;

namespace Dx29.Web.UI.Services
{
    public class NotificationService : IAsyncDisposable
    {
        static public bool IsInitialized { get; private set; }

        public NotificationService(NavigationManager navigationManager, IAccessTokenProvider accessTokenProvider, IMessageService messageServices, ILogService logService)
        {
            MessageServices = messageServices;
            LogService = logService;

            HubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/NotificationsHub"),
                options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var accessTokenResult = await accessTokenProvider.RequestAccessToken();
                        accessTokenResult.TryGetToken(out var accessToken);
                        return accessToken.Value;
                    };
                })
                .Build();
        }

        public IMessageService MessageServices { get; }
        public ILogService LogService { get; }

        public HubConnection HubConnection { get; set; }

        public async Task InitializeAsync()
        {
            await LogService.InfoAsync("InitializeAsync", "Previous Status {0}", IsInitialized);
            if (!IsInitialized)
            {
                IsInitialized = true;
                HubConnection.On<ReportInfo>("AnnotationsReady", OnAnnotationsReady);
                HubConnection.On<ReportInfo, string>("AnnotationsError", OnAnnotationsError);
                HubConnection.On<JobNotification>("Genotype", OnGenotypeNotification);
                HubConnection.On<string>("DataAnalysis", OnDataAnalysisNotification);
                await HubConnection.StartAsync();
            }
        }

        private void OnAnnotationsReady(ReportInfo reportInfo)
        {
            MessageServices.Send(this, "AnnotationsReady", reportInfo);
        }

        private void OnAnnotationsError(ReportInfo reportInfo, string message)
        {
            MessageServices.Send(this, "AnnotationsError", (reportInfo, message));
        }

        private void OnGenotypeNotification(JobNotification notification)
        {
            MessageServices.Send(this, "GenotypeNotification", notification);
        }

        private void OnDataAnalysisNotification(string message)
        {
            MessageServices.Send(this, "DataAnalysisNotification", message);
        }

        public async ValueTask DisposeAsync()
        {
            await LogService.InfoAsync("DisposeAsync", "Previous Status {0}", IsInitialized);
            if (IsInitialized)
            {
                IsInitialized = false;
                await HubConnection.DisposeAsync();
            }
        }
    }
}
