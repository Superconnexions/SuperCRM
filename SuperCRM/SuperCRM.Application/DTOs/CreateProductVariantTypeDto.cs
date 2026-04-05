using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCRM.Application.DTOs.ProductVariantTypes
{
    public class CreateProductVariantTypeDto
    {
        public string TypeCode { get; set; } = string.Empty;
        public string TypeValue { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}
