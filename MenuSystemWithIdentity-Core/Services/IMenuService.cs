using System.Collections.Generic;
using System.Security.Principal;
using MenuSystemWithIdentity_Core.Models;

namespace MenuSystemWithIdentity_Core.Services
{
    public interface IMenuService
    {
        public List<MenuItem> GetMenuByUser(IPrincipal user);
    }
}