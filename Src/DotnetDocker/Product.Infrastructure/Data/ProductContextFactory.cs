using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.Infrastructure.Data
{
    /*public class ProductContextFactory : IDesignTimeDbContextFactory<ProductContext>
    {
        public ProductContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProductContext>();
            optionsBuilder.UseSqlServer("server=192.168.1.104,14330;Initial Catalog=fzf003;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true");
             //.UseSqlite("Data Source=blog.db");
             return new ProductContext(optionsBuilder.Options);
        }
    }*/
}
