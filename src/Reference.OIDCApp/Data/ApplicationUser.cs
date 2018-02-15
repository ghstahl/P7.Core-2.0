using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using P7.AspNetCore.Identity.InMemory;

namespace Reference.OIDCApp.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationRole : MemoryRole
    {
    }

    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : MemoryUser
    {
    }
}
