using Product.Infrastructure.Domain;
using Product.Infrastructure.Dto;
using Product.Infrastructure.Repository;
using Product.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Application.Services
{
    public class ProductService : IProductService
    {
        readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }

        public async Task Active(long Id)
        {
            var product= await _productRepository.GetByIdAsync(Id);
            product.Activate();
           await _productRepository.UpdateAsync(product);
        }

        public async Task Discontinue(long Id)
        {
            var product = await _productRepository.GetByIdAsync(Id);
            product.Discontinue();
            await _productRepository.UpdateAsync(product);
        }

        public async Task<ProductDto> Create(ProductDto productModel)
        {
            var addproduct = ProductInfo.Create(productModel.ProductName, productModel.Category, productModel.Description, productModel.ImageFile, productModel.Price);
           
            var productinfo= await this._productRepository.AddAsync(addproduct);

            return new ProductDto()
            {
                ProductId = productinfo.Id,
                ProductName = productinfo.Name,
                Category = productinfo.Category,
                Price = productinfo.Price,
                Description = productinfo.Description,
                ImageFile = productinfo.ImageFile,
                Status = productinfo.Status,
                Summary = productinfo.Summary
            };
        }

        public async Task Delete(ProductDto productModel)
        {
            var product=await _productRepository.GetByIdAsync(productModel.ProductId);
            await _productRepository.DeleteAsync(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProductByCategory(string categoryId)
        {
            var cateoylist=  await _productRepository.GetAsync(p => p.Category == categoryId);
            return cateoylist.Select(productinfo => new ProductDto
            {
                ProductId = productinfo.Id,
                ProductName = productinfo.Name,
                Category = productinfo.Category,
                Price = productinfo.Price,
                Description = productinfo.Description,
                ImageFile = productinfo.ImageFile,
                Status = productinfo.Status,
                Summary = productinfo.Summary

            }).ToList();

        }

        public async Task<ProductDto> GetProductById(long productId)
        {
            var productinfo = await _productRepository.GetByIdAsync(productId);

            return new ProductDto
            {
                ProductId = productinfo.Id,
                ProductName = productinfo.Name,
                Category = productinfo.Category,
                Price = productinfo.Price,
                Description = productinfo.Description,
                ImageFile = productinfo.ImageFile,
                Status = productinfo.Status,
                Summary = productinfo.Summary

            };
        }

        public async Task<IEnumerable<ProductDto>> GetProductByName(string productName)
        {
            var products = await _productRepository.GetProductByName(productName);

            return products.Select(productinfo => new ProductDto
            {
                ProductId = productinfo.Id,
                ProductName = productinfo.Name,
                Category = productinfo.Category,
                Price = productinfo.Price,
                Description = productinfo.Description,
                ImageFile = productinfo.ImageFile,
                Status = productinfo.Status,
                Summary = productinfo.Summary

            });
        }

        public async Task<IEnumerable<ProductDto>> GetProductList()
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(productinfo => new ProductDto
            {
                ProductId = productinfo.Id,
                ProductName = productinfo.Name,
                Category = productinfo.Category,
                Price = productinfo.Price,
                Description = productinfo.Description,
                ImageFile = productinfo.ImageFile,
                Status = productinfo.Status,
                Summary = productinfo.Summary

            });

        }

        public async  Task UpdateName(ProductDto productModel)
        {
            var productinfo = await this._productRepository.GetByIdAsync(productModel.ProductId);
            productinfo.ChangeName(productModel.ProductName);
            await _productRepository.UpdateAsync(productinfo);
        }
    }
}
