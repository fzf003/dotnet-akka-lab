using Product.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.Application.Commands
{
    /// <summary>
    /// 创建商品草稿命令
    /// </summary>
    public class CreateProductDraftCommand
    {
         public ProductDto ProductDraft { get; set; }
    }
}
