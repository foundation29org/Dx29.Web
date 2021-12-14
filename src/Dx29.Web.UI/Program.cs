using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Dx29.Services;
using Dx29.Web.Services;
using Dx29.Web.UI.Services;

namespace Dx29.Web.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // Named HttpClient instance "Dx29.Web.API" without authentication token
            builder.Services.AddHttpClient("Dx29.Web.API", client => client.BaseAddress = BuildAPIAddress(builder));

            //Named HttpClient instance "Dx29.Web.API.Authenticated" with authentication token
            builder.Services.AddHttpClient("Dx29.Web.API.Authenticated", client => client.BaseAddress = BuildAPIAddress(builder))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Add ApiAuthorization
            builder.Services.AddApiAuthorization().AddAccountClaimsPrincipalFactory<RoleUserFactory>();

            // Add services
            builder.Services.AddScoped<AppState>();
            builder.Services.AddScoped<ILogService, LogService>();
            builder.Services.AddScoped<NavigationService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<DocumentService>();

            builder.Services.AddScoped<LocalizationService>();
            builder.Services.AddScoped<TimelineService>();
            builder.Services.AddScoped<Dx29Client>();

            // Add Localization
            builder.Services.AddLocalization();

            // Build and Run Host
            var webHost = builder.Build();
            await webHost.RunAsync();
        }

        private static Uri BuildAPIAddress(WebAssemblyHostBuilder builder)
        {
            return new Uri(String.Format("{0}{1}", builder.HostEnvironment.BaseAddress, "api/v1/"));
        }
    }
}
