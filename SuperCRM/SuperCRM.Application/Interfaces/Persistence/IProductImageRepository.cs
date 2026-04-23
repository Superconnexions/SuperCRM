
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface IProductImageRepository
    {
        Task<List<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task AddAsync(ProductImage entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
