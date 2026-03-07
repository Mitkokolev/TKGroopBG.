using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
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

        // ДОБАВИ ТЕЗИ ДВА РЕДА:
        // Това оправя грешка CS1061 в CartController (снимка image_0c8c11.png)
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;
    }
}
