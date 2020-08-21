using Product.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.Infrastructure.Dto
{
    public class ProductDto
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get;  set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get;   set; }
        /// <summary>
        /// 商品描述
        /// </summary>

        public string Description { get;   set; }
        /// <summary>
        /// 图片
        /// </summary>

        public string ImageFile { get;   set; }
        /// <summary>
        /// 商品价格
        /// </summary>

        public decimal? Price { get;   set; }
        /// <summary>
        /// 商品状态
        /// </summary>

        public ProductStatus Status { get;   set; }
    }
}
