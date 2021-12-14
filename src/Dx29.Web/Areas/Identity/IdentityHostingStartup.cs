using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Dx29.Web.Areas.Identity.IdentityHostingStartup))]
namespace Dx29.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}
