using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Models;

namespace TKGroopBG.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gallery> Gallery { get; set; } = default!;
        public DbSet<Products> Products { get; set; } = default!;

        public DbSet<Order> Orders { get; set; } = default!;
    }
}
