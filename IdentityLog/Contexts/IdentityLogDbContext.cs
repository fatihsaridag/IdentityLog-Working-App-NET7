using IdentityLog.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityLog.Contexts
{
    public class IdentityLogDbContext : IdentityDbContext<AppUser>
    {
        public IdentityLogDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
