using Identity.DataContexts;
using Identity.Models;
using Microsoft.AspNet.Http;
using System;

namespace Identity.Handlers
{
    public class AuditHandler
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        public AuditHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }
        public void LogAccess(string username, string password, string Activity)
        {
            try
            {
                SessionHandler test = new SessionHandler(HttpContextAccessor);

                test.SetSession("Marc", "Test");
                var cook = test.GetSession("Marc");
                using (var db = new CustomDatabase("Server=localhost;Database=CustomMembershipReboot;Trusted_Connection=True;"))
                {
                    var audit = new Logs
                    {
                        ID = new Guid(),
                        Date = DateTime.UtcNow,
                        Username = username,
                        Activity = Activity,
                        Detail = null,
                        PublicIP = "test",
                        PrivateIP = "test"
                    };
                    db.Audits.Add(audit);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}
