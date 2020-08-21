using Microsoft.AspNetCore.Mvc;
using Product.Infrastructure.Dto;
using Product.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            this._productService = productService;
        }

        

        [HttpGet("/create")]
        public Task<ProductDto> CreateProduct()
        {
            return this._productService.Create(new Infrastructure.Dto.ProductDto()
            {
                Category = "摄影师--陈曦",
                Summary = "如果一切正常，应能够向项目添加新的迁移",
                Description = "你可能想要将迁移存储在与包含你的 DbContext的程序集不同的程序集中。 你还可以使用此策略来维护多个迁移集，例如，一个用于开发，另一个用于发布到发布升级",
                ImageFile = "http://t7.baidu.com/it/u=747431361,2735836448&fm=79&app=86&f=JPEG?w=3682&h=2457",
                Price = 19.8m,
                ProductName = "摄影风采"
            });
        }


        [HttpGet("/prouctall")]
        public async Task<IEnumerable<ProductDto>> QueryProduct()
        {
            return await this._productService.GetProductList();
        }

        [HttpGet("/active/{Id}")]
        public   Task ProductActive(long Id)
        {
              return this._productService.Active(Id);
        }

        [HttpGet("/discontinue/{Id}")]
        public Task ProductDiscontinue(long Id)
        {
            return this._productService.Discontinue(Id);
        }



    }
}
