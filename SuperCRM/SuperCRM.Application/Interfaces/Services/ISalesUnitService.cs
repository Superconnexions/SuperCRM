using SuperCRM.Application.DTOs.SalesUnits;

namespace SuperCRM.Application.Interfaces.Services
{
    /// <summary>
    /// Application service contract for Sales Unit master setup.
    /// </summary>
    public interface ISalesUnitService
    {
        Task<List<SalesUnitDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SalesUnitDto?> GetByIdAsync(int salesUnitId, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> CreateAsync(CreateSalesUnitDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateSalesUnitDto request, CancellationToken cancellationToken = default);
    }
}
