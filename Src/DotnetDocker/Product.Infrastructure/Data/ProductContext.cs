using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Product.Infrastructure.Domain;
using ProductInfo = Product.Infrastructure.Domain.ProductInfo;

namespace Product.Infrastructure.Data
{
    public class ProductContext:DbContext
    {
        public ProductContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ProductInfo> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductInfo>()
               .Property(p => p.Timestamp)
               .IsRowVersion();
        }
    }
}