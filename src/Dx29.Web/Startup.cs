using System;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.DataProtection;

using Dx29.Services;
using Dx29.Web.Data;
using Dx29.Web.Models;
using Dx29.Web.Services;

namespace Dx29.Web
{
    public class Startup
    {
        public const string VERSION = "v0.15.2";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            // Add a DbContext to store your Database Keys
            services.AddDbContext<KeysDbContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            // using Microsoft.AspNetCore.DataProtection;
            services.AddDataProtection().PersistKeysToDbContext<KeysDbContext>();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser> (options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(2);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
                {
                    options.IdentityResources["openid"].UserClaims.Add(ClaimTypes.Email);
                    options.IdentityResources["openid"].UserClaims.Add("role");
                    options.ApiResources.Single().UserClaims.Add(ClaimTypes.Email);
                    options.ApiResources.Single().UserClaims.Add("role");
                });

            services.AddAuthentication().AddIdentityServerJwt();

            // Need to do this as it maps "role" to ClaimTypes.Role and causes issues
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // SignalR
            services.AddSignalR().AddAzureSignalR(Configuration["SignalR:ConnectionString"]);
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dx29 Web", Version = VERSION });
            });

            services.AddControllersWithViews();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddRazorPages()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                                    factory.Create(typeof(SharedResource));
                });

            services.AddApplicationInsightsTelemetry(Configuration["AppInsights:Key"]);

            // Add Dx29 Services
            services.AddServices(Configuration);
            services.AddWebServices(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"Dx29 Web {VERSION}"));

#if  !DEBUG
            app.Use(async (ctx, next) =>
            {
                ctx.Request.Scheme = "https";
                ctx.Request.Host = new Microsoft.AspNetCore.Http.HostString(Configuration["Application:Host"]);

                await next();
            });
#endif

            var supportedCultures = new[] { "en", "es" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<NotificationsHub>("/NotificationsHub");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
