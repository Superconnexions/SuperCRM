
using SuperCRM.Application.DTOs.Products;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IProductImageService
    {
        Task<(bool Success, string ErrorMessage)> AddAsync(CreateProductImageDto request, CancellationToken cancellationToken = default);
    }
}
