using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MenuSystemWithIdentity_Core.Identity
{
    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole() { }
        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
            RoleMenus = new HashSet<ApplicationRoleMenu>();
        }

        public ICollection<ApplicationRoleMenu> RoleMenus { get; set; }

    }
}

