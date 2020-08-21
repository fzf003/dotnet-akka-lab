using Product.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.Application.Events
{
    public class CreateProductEvent
    {
        public ProductDto ProductDraft { get; set; }
    }
}
