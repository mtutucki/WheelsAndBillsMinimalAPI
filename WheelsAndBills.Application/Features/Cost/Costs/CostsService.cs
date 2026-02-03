using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using CostEntity = WheelsAndBills.Domain.Entities.Cost.Cost;
using static WheelsAndBills.Application.DTOs.Costs.CostsDTO;

namespace WheelsAndBills.Application.Features.Cost.Costs
{
    public class CostsService : ICostsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorEventMissing = "VehicleEvent does not exist";
        private const string ErrorCostTypeMissing = "CostType does not exist";

        private readonly IAppDbContext _db;

        public CostsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetCostDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Costs
                .OrderByDescending(c => c.Amount)
                .Select(c => new GetCostDTO(
                    c.Id,
                    c.VehicleEventId,
                    c.CostTypeId,
                    c.Amount
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetCostDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Costs
                .Where(c => c.Id == id)
                .Select(c => new GetCostDTO(
                    c.Id,
                    c.VehicleEventId,
                    c.CostTypeId,
                    c.Amount
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetCostDTO>> CreateAsync(CreateCostDTO request, CancellationToken cancellationToken = default)
        {
            var eventExists = await _db.VehicleEvents
                .AnyAsync(e => e.Id == request.VehicleEventId, cancellationToken);
            if (!eventExists)
                return ServiceResult<GetCostDTO>.Fail(ErrorEventMissing);

            var costTypeExists = await _db.CostTypes
                .AnyAsync(ct => ct.Id == request.CostTypeId, cancellationToken);
            if (!costTypeExists)
                return ServiceResult<GetCostDTO>.Fail(ErrorCostTypeMissing);

            var cost = new CostEntity
            {
                Id = Guid.NewGuid(),
                VehicleEventId = request.VehicleEventId,
                CostTypeId = request.CostTypeId,
                Amount = request.Amount
            };

            _db.Costs.Add(cost);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetCostDTO>.Ok(new GetCostDTO(
                cost.Id,
                cost.VehicleEventId,
                cost.CostTypeId,
                cost.Amount
            ));
        }

        public async Task<ServiceResult<GetCostDTO>> UpdateAsync(Guid id, UpdateCostDTO request, CancellationToken cancellationToken = default)
        {
            var cost = await _db.Costs.FindAsync(new object?[] { id }, cancellationToken);
            if (cost is null)
                return ServiceResult<GetCostDTO>.Fail(ErrorNotFound);

            var costTypeExists = await _db.CostTypes
                .AnyAsync(ct => ct.Id == request.CostTypeId, cancellationToken);
            if (!costTypeExists)
                return ServiceResult<GetCostDTO>.Fail(ErrorCostTypeMissing);

            cost.CostTypeId = request.CostTypeId;
            cost.Amount = request.Amount;

            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetCostDTO>.Ok(new GetCostDTO(
                cost.Id,
                cost.VehicleEventId,
                cost.CostTypeId,
                cost.Amount
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cost = await _db.Costs.FindAsync(new object?[] { id }, cancellationToken);
            if (cost is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.Costs.Remove(cost);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
