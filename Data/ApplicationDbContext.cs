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
        public DbSet<TKGroopBG.Models.Gallery> Gallery { get; set; } = default!;
        public DbSet<TKGroopBG.Models.Products> Products { get; set; } = default!;
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Order { get; set; }

    }
}
