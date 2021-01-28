using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Product.Infrastructure.Domain
{
   
    [Table("Product")]
    public class ProductInfo: Entity
    {

        /// <summary>
        /// 商品名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; private set; }
        /// <summary>
        /// 类别
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Category { get; private set; }
        /// <summary>
        /// 摘要
        /// </summary>
        [Required]
        [StringLength(520)]
        public string Summary { get; private set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string Description { get; private set; }
        /// <summary>
        /// 图片
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ImageFile { get; private set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        
        public decimal? Price { get; private set; }
        /// <summary>
        /// 商品状态
        /// </summary>
        [Required]
        public ProductStatus Status { get; private set; }

        [Required]
        public DateTime CreateTime { get; private set; }

        [Timestamp]
        //[Required]
        public byte[] Timestamp { get; private set; }

        private ProductInfo() { }

        private ProductInfo(string name ,string category,string description,string imagefile,decimal? price)
        {
            this.Name = name;
            this.Summary = category;
            this.Category = category;
            this.Description = description;
            this.ImageFile = imagefile;
            this.Price = price;
            this.Status = ProductStatus.Draft;
            this.CreateTime = DateTime.Now;
        }

        public void ChangeName(string name)
        {
            this.Name = name;
            this.CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 激活
        /// </summary>
        public void Activate()
        {
            this.Status = ProductStatus.Active;
            this.CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 停售
        /// </summary>
        public void Discontinue()
        {
            this.Status = ProductStatus.Discontinued;
            this.CreateTime = DateTime.Now;
        }

        public static ProductInfo Create(string name, string category, string description,string image, decimal? price)
        {
            return new ProductInfo(name, category, description,image, price);
        }
    }

    public enum ProductStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        Draft,
        /// <summary>
        /// 已激活
        /// </summary>
        Active,
        /// <summary>
        /// 禁售
        /// </summary>
        Discontinued
    }
}
