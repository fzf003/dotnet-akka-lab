using Microsoft.EntityFrameworkCore;
using Product.Infrastructure.Data;
using Product.Infrastructure.Domain;
using Product.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Product.Application.Repository
{
    public class ProductRepository : Repository<ProductInfo>, IProductRepository
    {
        public ProductRepository(ProductContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<ProductInfo>> GetProductByName(string name)
        {
            return await _dbContext.Products.Where(p => p.Name == name).ToListAsync();
        }
    }
}
