using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using CostTypeEntity = WheelsAndBills.Domain.Entities.Cost.CostType;
using static WheelsAndBills.Application.DTOs.Costs.CostsDTO;

namespace WheelsAndBills.Application.Features.Cost.CostTypes
{
    public class CostTypesService : ICostTypesService
    {
        private const string ErrorDuplicate = "Cost type already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public CostTypesService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetCostTypeDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.CostTypes
                .OrderBy(t => t.Name)
                .Select(t => new GetCostTypeDTO(t.Id, t.Name))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetCostTypeDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.CostTypes
                .Where(t => t.Id == id)
                .Select(t => new GetCostTypeDTO(t.Id, t.Name))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetCostTypeDTO>> CreateAsync(CreateCostTypeDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.CostTypes
                .AnyAsync(t => t.Name == request.Name, cancellationToken);
            if (exists)
                return ServiceResult<GetCostTypeDTO>.Fail(ErrorDuplicate);

            var type = new CostTypeEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _db.CostTypes.Add(type);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetCostTypeDTO>.Ok(new GetCostTypeDTO(type.Id, type.Name));
        }

        public async Task<ServiceResult<GetCostTypeDTO>> UpdateAsync(Guid id, UpdateCostTypeDTO request, CancellationToken cancellationToken = default)
        {
            var type = await _db.CostTypes.FindAsync(new object?[] { id }, cancellationToken);
            if (type is null)
                return ServiceResult<GetCostTypeDTO>.Fail(ErrorNotFound);

            var exists = await _db.CostTypes
                .AnyAsync(t => t.Name == request.Name && t.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetCostTypeDTO>.Fail(ErrorDuplicate);

            type.Name = request.Name;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetCostTypeDTO>.Ok(new GetCostTypeDTO(type.Id, type.Name));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var type = await _db.CostTypes.FindAsync(new object?[] { id }, cancellationToken);
            if (type is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.CostTypes.Remove(type);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
