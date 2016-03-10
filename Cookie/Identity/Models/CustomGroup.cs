﻿using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using Identity.Configurations;
using Identity.DataContexts;

namespace Identity.Models
{
    public class CustomGroup : RelationalGroup
    {
        public virtual string Description { get; set; }
    }

    public class CustomGroupService : GroupService<CustomGroup>
    {
        public CustomGroupService(CustomGroupRepository repo, CustomConfig config)
            : base(config.DefaultTenant, repo)
        {

        }
    }

    public class CustomGroupRepository : DbContextGroupRepository<CustomDatabase, CustomGroup>
    {
        public CustomGroupRepository(CustomDatabase ctx)
            : base(ctx)
        {
        }
    }

}
