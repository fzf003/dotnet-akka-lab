using Product.Infrastructure.Data;
using Product.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Product.Infrastructure.Repository
{
    public interface IProductRepository: IRepository<ProductInfo>
    {
        
        Task<IEnumerable<ProductInfo>> GetProductByName(string name);
        
    }
}
