using SuperCRM.Application.DTOs.SalesUnits;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Services
{
    /// <summary>
    /// Standard application service for Sales Unit master setup.
    /// Handles validation, uniqueness checks, normalization, and mapping.
    /// </summary>
    public class SalesUnitService : ISalesUnitService
    {
        private readonly ISalesUnitRepository _repository;

        public SalesUnitService(ISalesUnitRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Returns all sales units ordered by UnitName.
        /// </summary>
        public async Task<List<SalesUnitDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);

            return entities
                .OrderBy(x => x.UnitName)
                .Select(MapToDto)
                .ToList();
        }

        /// <summary>
        /// Loads one sales unit by primary key.
        /// </summary>
        public async Task<SalesUnitDto?> GetByIdAsync(int salesUnitId, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(salesUnitId, cancellationToken);
            return entity == null ? null : MapToDto(entity);
        }

        /// <summary>
        /// Creates a new sales unit after validation and normalization.
        /// UnitCode is normalized to uppercase trimmed format.
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> CreateAsync(CreateSalesUnitDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.UnitCode))
                return (false, "Unit Code is required.");

            if (string.IsNullOrWhiteSpace(request.UnitName))
                return (false, "Unit Name is required.");

            var normalizedCode = request.UnitCode.Trim().ToUpperInvariant();
            var normalizedName = request.UnitName.Trim();

            if (await _repository.ExistsByCodeAsync(normalizedCode, null, cancellationToken))
                return (false, "Unit Code already exists.");

            if (await _repository.ExistsByNameAsync(normalizedName, null, cancellationToken))
                return (false, "Unit Name already exists.");

            var entity = new SalesUnit
            {
                UnitCode = normalizedCode,
                UnitName = normalizedName,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                UpdatedByUserId = request.UpdatedByUserId
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }

        /// <summary>
        /// Updates an existing sales unit after validation.
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateSalesUnitDto request, CancellationToken cancellationToken = default)
        {
            if (request.SalesUnitId <= 0)
                return (false, "Invalid Sales Unit Id.");

            if (string.IsNullOrWhiteSpace(request.UnitCode))
                return (false, "Unit Code is required.");

            if (string.IsNullOrWhiteSpace(request.UnitName))
                return (false, "Unit Name is required.");

            var entity = await _repository.GetByIdAsync(request.SalesUnitId, cancellationToken);
            if (entity == null)
                return (false, "Sales Unit not found.");

            var normalizedCode = request.UnitCode.Trim().ToUpperInvariant();
            var normalizedName = request.UnitName.Trim();

            if (await _repository.ExistsByCodeAsync(normalizedCode, request.SalesUnitId, cancellationToken))
                return (false, "Unit Code already exists.");

            if (await _repository.ExistsByNameAsync(normalizedName, request.SalesUnitId, cancellationToken))
                return (false, "Unit Name already exists.");

            entity.UnitCode = normalizedCode;
            entity.UnitName = normalizedName;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = request.UpdatedByUserId;

            await _repository.UpdateAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }

        private static SalesUnitDto MapToDto(SalesUnit entity)
        {
            return new SalesUnitDto
            {
                SalesUnitId = entity.SalesUnitId,
                UnitCode = entity.UnitCode,
                UnitName = entity.UnitName,
                IsActive = entity.IsActive
            };
        }
    }
}
