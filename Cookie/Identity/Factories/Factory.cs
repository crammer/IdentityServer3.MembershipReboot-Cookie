using Identity.Models;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.EntityFramework;
using IdentityServer3.Core.Models;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;

namespace Identity.Factories
{
    class Factory
    {
        public static IdentityServerServiceFactory Configure(string connectionString, string schema, IApplicationEnvironment env)
        {
            var factory = new IdentityServerServiceFactory();

            var IdentityServerEntityFrameworkConfiguration = new EntityFrameworkServiceOptions
            {
                ConnectionString = connectionString,
                Schema = schema
            };

            ConfigureClients(Clients.Get(), IdentityServerEntityFrameworkConfiguration);
            ConfigureScopes(Scopes.Get(), IdentityServerEntityFrameworkConfiguration);

            factory.RegisterConfigurationServices(IdentityServerEntityFrameworkConfiguration);
            factory.RegisterOperationalServices(IdentityServerEntityFrameworkConfiguration);

            var viewOptions = new DefaultViewServiceOptions
            {
                CacheViews = false,
                ViewLoader = new Registration<IViewLoader>(new FileSystemWithEmbeddedFallbackViewLoader(Path.Combine(env.ApplicationBasePath, "templates")))
            };

            viewOptions.CacheViews = false;

            factory.ConfigureDefaultViewService(viewOptions);


            factory.CorsPolicyService = new Registration<ICorsPolicyService>(new DefaultCorsPolicyService { AllowAll = true });

            return factory;
        }

        public static void ConfigureClients(IEnumerable<Client> clients, EntityFrameworkServiceOptions options)
        {
            using (var db = new ClientConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Clients.Any())
                {
                    foreach (Client c in clients)
                    {
                        IdentityServer3.EntityFramework.Entities.Client e = c.ToEntity();
                        db.Clients.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }

        public static void ConfigureScopes(IEnumerable<Scope> scopes, EntityFrameworkServiceOptions options)
        {
            using (var db = new ScopeConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Scopes.Any())
                {
                    foreach (Scope s in scopes)
                    {
                        IdentityServer3.EntityFramework.Entities.Scope e = s.ToEntity();
                        db.Scopes.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }

    }
}
