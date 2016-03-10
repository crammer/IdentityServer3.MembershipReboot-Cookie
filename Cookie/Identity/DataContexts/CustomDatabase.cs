using BrockAllen.MembershipReboot.Ef;
using Identity.Models;
using System.Data.Entity;

namespace Identity.DataContexts
{
    public class CustomDatabase : MembershipRebootDbContext<CustomUser, CustomGroup>
    {
        public CustomDatabase(string name)
            : base(name)
        {
        }

        public DbSet<Logs> Audits
        {
            get;
            set;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureMembershipRebootUserAccounts<CustomUser>();
            modelBuilder.ConfigureMembershipRebootGroups<CustomGroup>();
        }
    }
}
