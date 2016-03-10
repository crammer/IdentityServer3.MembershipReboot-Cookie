using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using IdentityServer3.Core.Configuration;
using Autofac;
using System;
using Autofac.Extensions.DependencyInjection;
using Identity.Handlers;
using Identity.Factories;
using Identity.Services;

namespace Identity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCaching();
            services.AddSession();
            services.AddTransient<SessionHandler>();

            // Add framework services.
            services.AddMvc();
            //services.AddDataProtection();
            //services.AddScoped<AuditHandler>();

            // Create the Autofac container builder.
            var builder = new ContainerBuilder();

            // Add any Autofac modules or registrations.
            builder.RegisterModule(new AutofacModule());

            // Populate the services.
            builder.Populate(services);

            // Build the container.
            var container = builder.Build();

            // Resolve and return the service provider.
            return container.Resolve<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app
            , IApplicationEnvironment appEnv
            , ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();


            app.UseSession();

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseDeveloperExceptionPage();

            var certFile = appEnv.ApplicationBasePath + "\\Certificates\\idsrv3test.pfx";

            app.Map("", core =>
            {
                var serverConfiguration = Factory.Configure(Configuration["Data:DefaultConnection:ConnectionString"], "Authorization", appEnv);
                serverConfiguration.ConfigureCustomUserService(Configuration["Data:DefaultConnection:ConnectionString"]);

                var options = new IdentityServerOptions
                {
                    IssuerUri = "https://identityserver.com",
                    SiteName = "IdentityServer",
                    SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                    EnableWelcomePage = false,
                    Factory = serverConfiguration,
                    RequireSsl = false,
                    CspOptions = new CspOptions
                    {
                        Enabled = true,
                        ScriptSrc =
                            "'unsafe-inline' 'self' fonts.googleapis.com www.googleapis.com google-code-prettify.googlecode.com cdnjs.cloudflare.com oss.maxcdn.com ip-api.com",
                        StyleSrc =
                            "'self' fonts.googleapis.com fonts.gstatic.com www.googleapis.com cdnjs.cloudflare.com; font-src 'self' fonts.googleapis.com fonts.gstatic.com",
                        ConnectSrc =
                            "default-src 'self' maps.googleapis.com freegeoip.net;"
                    },

                };

                core.UseIdentityServer(options);
            });

        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
