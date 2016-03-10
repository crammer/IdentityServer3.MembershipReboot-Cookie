using Autofac;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Owin;
using Identity.DataContexts;
using Identity.Models;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;

namespace Identity
{
    public class AutofacModule : Autofac.Module
    {

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<CustomDatabase>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CustomUserRepository>()
                .As<IUserAccountRepository<CustomUser>>()
                .As<IUserAccountQuery<CustomUser>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CustomGroupRepository>()
                .As<IGroupRepository<CustomGroup>>()
                .As<IGroupQuery>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GroupService<CustomGroup>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<UserAccountService<CustomUser>>().OnActivating(e =>
            {
                var owin = e.Context.Resolve<IOwinContext>();
                bool debugging = false;
#if DEBUG
                debugging = true;
#endif
                e.Instance.ConfigureTwoFactorAuthenticationCookies(owin.Environment, debugging);
            })
                .AsSelf()
                .InstancePerLifetimeScope();

            var cookieOptions = new CookieAuthenticationOptions
            {
                AuthenticationType = MembershipRebootOwinConstants.AuthenticationType
            };

            builder.Register(ctx =>
            {
                var owin = ctx.Resolve<IOwinContext>();
                return new OwinAuthenticationService<CustomUser>(cookieOptions.AuthenticationType,
                    ctx.Resolve<UserAccountService<CustomUser>>(),
                    owin.Environment);
            })
                .As<AuthenticationService<CustomUser>>()
                .InstancePerLifetimeScope();

        }
    }
}
