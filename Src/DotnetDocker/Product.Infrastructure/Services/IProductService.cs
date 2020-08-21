using Product.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Product.Infrastructure.Services
{
    public interface IProductService
    {

        Task<IEnumerable<ProductDto>> GetProductList();
        Task<ProductDto> GetProductById(long productId);
        Task<IEnumerable<ProductDto>> GetProductByName(string productName);
        Task<IEnumerable<ProductDto>> GetProductByCategory(string categoryId);
        Task<ProductDto> Create(ProductDto productModel);
        Task UpdateName(ProductDto productModel);
        Task Delete(ProductDto productModel);
        Task Discontinue(long Id);
        Task Active(long Id);
    }
}
