using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;

namespace Dx29.Web.Services
{
    static public class WebServiceConfiguration
    {
        static public void AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<UserServices>();
            services.AddSingleton<IUserIdProvider, NotificationsIdProvider>();
            services.AddTransient<OpenDataService>();
        }
    }
}
