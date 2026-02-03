using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using RecurringCostEntity = WheelsAndBills.Domain.Entities.Cost.RecurringCost;
using static WheelsAndBills.Application.DTOs.Costs.CostsDTO;

namespace WheelsAndBills.Application.Features.Cost.RecurringCosts
{
    public class RecurringCostsService : IRecurringCostsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorVehicleMissing = "Vehicle does not exist";
        private const string ErrorCostTypeMissing = "CostType does not exist";
        private const string ErrorInterval = "IntervalMonths must be greater than 0";

        private readonly IAppDbContext _db;

        public RecurringCostsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetRecurringCostDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.RecurringCosts
                .OrderBy(c => c.IntervalMonths)
                .Select(c => new GetRecurringCostDTO(
                    c.Id,
                    c.VehicleId,
                    c.CostTypeId,
                    c.Amount,
                    c.IntervalMonths
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetRecurringCostDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.RecurringCosts
                .Where(c => c.Id == id)
                .Select(c => new GetRecurringCostDTO(
                    c.Id,
                    c.VehicleId,
                    c.CostTypeId,
                    c.Amount,
                    c.IntervalMonths
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetRecurringCostDTO>> CreateAsync(CreateRecurringCostDTO request, CancellationToken cancellationToken = default)
        {
            var vehicleExists = await _db.Vehicles
                .AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
            if (!vehicleExists)
                return ServiceResult<GetRecurringCostDTO>.Fail(ErrorVehicleMissing);

            var costTypeExists = await _db.CostTypes
                .AnyAsync(ct => ct.Id == request.CostTypeId, cancellationToken);
            if (!costTypeExists)
                return ServiceResult<GetRecurringCostDTO>.Fail(ErrorCostTypeMissing);

            if (request.IntervalMonths <= 0)
                return ServiceResult<GetRecurringCostDTO>.Fail(ErrorInterval);

            var cost = new RecurringCostEntity
            {
                Id = Guid.NewGuid(),
                VehicleId = request.VehicleId,
                CostTypeId = request.CostTypeId,
                Amount = request.Amount,
                IntervalMonths = request.IntervalMonths
            };

            _db.RecurringCosts.Add(cost);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetRecurringCostDTO>.Ok(new GetRecurringCostDTO(
                cost.Id,
                cost.VehicleId,
                cost.CostTypeId,
                cost.Amount,
                cost.IntervalMonths
            ));
        }

        public async Task<ServiceResult<GetRecurringCostDTO>> UpdateAsync(Guid id, UpdateRecurringCostDTO request, CancellationToken cancellationToken = default)
        {
            var cost = await _db.RecurringCosts.FindAsync(new object?[] { id }, cancellationToken);
            if (cost is null)
                return ServiceResult<GetRecurringCostDTO>.Fail(ErrorNotFound);

            var costTypeExists = await _db.CostTypes
                .AnyAsync(ct => ct.Id == request.CostTypeId, cancellationToken);
            if (!costTypeExists)
                return ServiceResult<GetRecurringCostDTO>.Fail(ErrorCostTypeMissing);

            if (request.IntervalMonths <= 0)
                return ServiceResult<GetRecurringCostDTO>.Fail(ErrorInterval);

            cost.CostTypeId = request.CostTypeId;
            cost.Amount = request.Amount;
            cost.IntervalMonths = request.IntervalMonths;

            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetRecurringCostDTO>.Ok(new GetRecurringCostDTO(
                cost.Id,
                cost.VehicleId,
                cost.CostTypeId,
                cost.Amount,
                cost.IntervalMonths
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cost = await _db.RecurringCosts.FindAsync(new object?[] { id }, cancellationToken);
            if (cost is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.RecurringCosts.Remove(cost);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
