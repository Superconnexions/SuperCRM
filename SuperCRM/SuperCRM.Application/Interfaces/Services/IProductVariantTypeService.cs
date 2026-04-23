using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperCRM.Application.DTOs.ProductVariantTypes;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IProductVariantTypeService
    {
        Task<List<ProductVariantTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductVariantTypeDto?> GetByTypeCodeAsync(string typeCode, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductVariantTypeDto dto, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductVariantTypeDto dto, CancellationToken cancellationToken = default);
    }
}
