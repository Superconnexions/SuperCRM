using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface IProductVariantTypeRepository
    {
        Task<List<ProductVariantType>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductVariantType?> GetByTypeCodeAsync(string typeCode, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTypeCodeAsync(string typeCode, CancellationToken cancellationToken = default);
        Task AddAsync(ProductVariantType entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductVariantType entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
