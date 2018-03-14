using Microsoft.EntityFrameworkCore;

namespace ShopifyApp.Models
{
    public class ShopifyContext : DbContext
    {
        public DbSet<Shop> Shops { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Shopify;Integrated Security=True");
        }
    }
}
