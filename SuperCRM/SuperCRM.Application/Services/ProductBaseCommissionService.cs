using SuperCRM.Application.DTOs.ProductBaseCommissions;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.Services
{
    public class ProductBaseCommissionService : IProductBaseCommissionService
    {
        private readonly IProductBaseCommissionRepository _repository;

        public ProductBaseCommissionService(IProductBaseCommissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductBaseCommissionDto>> SearchAsync(ProductBaseCommissionSearchDto search, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.SearchAsync(search.ProductKeyword, search.EffectiveFrom, search.EffectiveTo, search.IncludeInactive, cancellationToken);
            return entities.Select(MapToDto).ToList();
        }

        public async Task<ProductBaseCommissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<List<ProductBaseCommissionHistoryDto>> GetHistoryAsync(Guid productBaseCommissionId, CancellationToken cancellationToken = default)
        {
            var histories = await _repository.GetHistoryByCommissionIdAsync(productBaseCommissionId, cancellationToken);
            return histories.Select(x => new ProductBaseCommissionHistoryDto
            {
                HistoryId = x.HistoryId,
                ProductBaseCommissionId = x.ProductBaseCommissionId,
                ProductId = x.ProductId,
                ProductCode = x.Product?.ProductCode ?? string.Empty,
                ProductName = x.Product?.ProductName ?? string.Empty,
                OldCommissionType = x.OldCommissionType,
                OldFixedAmount = x.OldFixedAmount,
                OldPercentage = x.OldPercentage,
                NewCommissionType = x.NewCommissionType,
                NewFixedAmount = x.NewFixedAmount,
                NewPercentage = x.NewPercentage,
                ChangedAt = x.ChangedAt,
                Note = x.Note
            }).ToList();
        }

        public Task<List<ProductLookupDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            return _repository.GetActiveProductsAsync(cancellationToken);
        }

        public async Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductBaseCommissionDto request, CancellationToken cancellationToken = default)
        {
            var validation = await ValidateAsync(request.ProductId, request.CommissionType, request.FixedAmount, request.Percentage, request.EffectiveFrom, request.EffectiveTo, null, cancellationToken);
            if (!validation.Success)
                return validation;

            var entity = new ProductBaseCommission
            {
                ProductBaseCommissionId = Guid.NewGuid(),
                ProductId = request.ProductId,
                CommissionType = request.CommissionType,
                FixedAmount = NormalizeFixedAmount(request.CommissionType, request.FixedAmount),
                Percentage = NormalizePercentage(request.CommissionType, request.Percentage),
                IsActive = true,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = request.CreatedByUserId
            };

            await _repository.AddAsync(entity, cancellationToken);

            await _repository.AddHistoryAsync(new ProductBaseCommissionHistory
            {
                HistoryId = Guid.NewGuid(),
                ProductBaseCommissionId = entity.ProductBaseCommissionId,
                ProductId = entity.ProductId,
                OldCommissionType = null,
                OldFixedAmount = null,
                OldPercentage = null,
                NewCommissionType = entity.CommissionType,
                NewFixedAmount = entity.FixedAmount,
                NewPercentage = entity.Percentage,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = request.CreatedByUserId,
                Note = string.IsNullOrWhiteSpace(request.Note) ? "Created" : request.Note,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = request.CreatedByUserId
            }, cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);
            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductBaseCommissionDto request, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(request.ProductBaseCommissionId, cancellationToken);
            if (entity == null)
                return (false, "Product base commission not found.");

            var validation = await ValidateAsync(request.ProductId, request.CommissionType, request.FixedAmount, request.Percentage, request.EffectiveFrom, request.EffectiveTo, request.ProductBaseCommissionId, cancellationToken);
            if (!validation.Success)
                return validation;

            var history = new ProductBaseCommissionHistory
            {
                HistoryId = Guid.NewGuid(),
                ProductBaseCommissionId = entity.ProductBaseCommissionId,
                ProductId = request.ProductId,
                OldCommissionType = entity.CommissionType,
                OldFixedAmount = entity.FixedAmount,
                OldPercentage = entity.Percentage,
                NewCommissionType = request.CommissionType,
                NewFixedAmount = NormalizeFixedAmount(request.CommissionType, request.FixedAmount),
                NewPercentage = NormalizePercentage(request.CommissionType, request.Percentage),
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = request.UpdatedByUserId,
                Note = string.IsNullOrWhiteSpace(request.Note) ? "Updated" : request.Note,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = request.UpdatedByUserId
            };

            entity.ProductId = request.ProductId;
            entity.CommissionType = request.CommissionType;
            entity.FixedAmount = history.NewFixedAmount;
            entity.Percentage = history.NewPercentage;
            entity.EffectiveFrom = request.EffectiveFrom;
            entity.EffectiveTo = request.EffectiveTo;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = request.UpdatedByUserId;

            await _repository.UpdateAsync(entity, cancellationToken);
            await _repository.AddHistoryAsync(history, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> SoftDeleteAsync(Guid id, Guid changedByUserId, string? note, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return (false, "Product base commission not found.");

            if (!entity.IsActive)
                return (false, "Product base commission is already inactive.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = changedByUserId;

            await _repository.UpdateAsync(entity, cancellationToken);

            await _repository.AddHistoryAsync(new ProductBaseCommissionHistory
            {
                HistoryId = Guid.NewGuid(),
                ProductBaseCommissionId = entity.ProductBaseCommissionId,
                ProductId = entity.ProductId,
                OldCommissionType = entity.CommissionType,
                OldFixedAmount = entity.FixedAmount,
                OldPercentage = entity.Percentage,
                NewCommissionType = entity.CommissionType,
                NewFixedAmount = entity.FixedAmount,
                NewPercentage = entity.Percentage,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = changedByUserId,
                Note = string.IsNullOrWhiteSpace(note) ? "Soft deleted (inactive)" : note,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = changedByUserId
            }, cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);
            return (true, string.Empty);
        }

        public async Task<ProductBaseCommissionDto?> GetSmartCommissionAsync(Guid productId, DateTime orderDate, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetSmartCommissionAsync(productId, orderDate, cancellationToken);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<CommissionCalculationResultDto> CalculateCommissionAsync(Guid productId, DateTime orderDate, decimal orderAmount, CancellationToken cancellationToken = default)
        {
            var commission = await _repository.GetSmartCommissionAsync(productId, orderDate, cancellationToken);
            if (commission == null)
            {
                return new CommissionCalculationResultDto
                {
                    Found = false,
                    Message = "No active commission found for the selected product and order date."
                };
            }

            decimal amount = 0;
            if (commission.CommissionType == CommissionType.FixedAmount)
            {
                amount = commission.FixedAmount ?? 0;
            }
            else if (commission.CommissionType == CommissionType.Percentage)
            {
                amount = orderAmount * (commission.Percentage ?? 0) / 100m;
            }

            return new CommissionCalculationResultDto
            {
                Found = true,
                ProductBaseCommissionId = commission.ProductBaseCommissionId,
                CommissionType = commission.CommissionType,
                FixedAmount = commission.FixedAmount,
                Percentage = commission.Percentage,
                CommissionAmount = amount,
                Message = "Commission calculated successfully."
            };
        }

        private async Task<(bool Success, string ErrorMessage)> ValidateAsync(
            Guid productId,
            CommissionType commissionType,
            decimal? fixedAmount,
            decimal? percentage,
            DateTime? effectiveFrom,
            DateTime? effectiveTo,
            Guid? excludeCommissionId,
            CancellationToken cancellationToken)
        {
            if (productId == Guid.Empty)
                return (false, "Product is required.");

            var product = await _repository.GetProductByIdAsync(productId, cancellationToken);
            if (product == null)
                return (false, "Selected product was not found.");

            if (effectiveFrom.HasValue && effectiveTo.HasValue && effectiveFrom > effectiveTo)
                return (false, "Effective To must be greater than or equal to Effective From.");

            if (commissionType == CommissionType.FixedAmount)
            {
                if (!fixedAmount.HasValue || fixedAmount.Value < 0)
                    return (false, "Fixed amount is required for FixedAmount commission type.");

                if (percentage.HasValue && percentage.Value != 0)
                    return (false, "Percentage must be empty or zero for FixedAmount commission type.");
            }
            else if (commissionType == CommissionType.Percentage)
            {
                if (!percentage.HasValue || percentage.Value < 0)
                    return (false, "Percentage is required for Percentage commission type.");

                if (percentage.Value > 100)
                    return (false, "Percentage cannot be greater than 100.");

                if (fixedAmount.HasValue && fixedAmount.Value != 0)
                    return (false, "Fixed amount must be empty or zero for Percentage commission type.");
            }

            var hasOverlap = await _repository.ExistsOverlappingActiveCommissionAsync(productId, effectiveFrom, effectiveTo, excludeCommissionId, cancellationToken);
            if (hasOverlap)
                return (false, "Another active commission already exists for this product within the selected effective date range.");

            return (true, string.Empty);
        }

        private static decimal? NormalizeFixedAmount(CommissionType type, decimal? fixedAmount)
            => type == CommissionType.FixedAmount ? fixedAmount : null;

        private static decimal? NormalizePercentage(CommissionType type, decimal? percentage)
            => type == CommissionType.Percentage ? percentage : null;

        private static ProductBaseCommissionDto MapToDto(ProductBaseCommission x)
        {
            return new ProductBaseCommissionDto
            {
                ProductBaseCommissionId = x.ProductBaseCommissionId,
                ProductId = x.ProductId,
                ProductCode = x.Product?.ProductCode ?? string.Empty,
                ProductName = x.Product?.ProductName ?? string.Empty,
                CommissionType = x.CommissionType,
                FixedAmount = x.FixedAmount,
                Percentage = x.Percentage,
                IsActive = x.IsActive,
                EffectiveFrom = x.EffectiveFrom,
                EffectiveTo = x.EffectiveTo,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            };
        }
    }
}
