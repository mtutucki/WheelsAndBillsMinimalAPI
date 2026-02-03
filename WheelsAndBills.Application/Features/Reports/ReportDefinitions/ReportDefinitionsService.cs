using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using ReportDefinitionEntity = WheelsAndBills.Domain.Entities.Report.ReportDefinition;
using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;

namespace WheelsAndBills.Application.Features.Reports.ReportDefinitions
{
    public class ReportDefinitionsService : IReportDefinitionsService
    {
        private const string ErrorDuplicate = "ReportDefinition already exists";
        private const string ErrorNotFound = "NotFound";

        private readonly IAppDbContext _db;

        public ReportDefinitionsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetReportDefinitionDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.ReportDefinitions
                .OrderBy(d => d.Code)
                .Select(d => new GetReportDefinitionDTO(d.Id, d.Code))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetReportDefinitionDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.ReportDefinitions
                .Where(d => d.Id == id)
                .Select(d => new GetReportDefinitionDTO(d.Id, d.Code))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetReportDefinitionDTO>> CreateAsync(CreateReportDefinitionDTO request, CancellationToken cancellationToken = default)
        {
            var exists = await _db.ReportDefinitions
                .AnyAsync(d => d.Code == request.Code, cancellationToken);
            if (exists)
                return ServiceResult<GetReportDefinitionDTO>.Fail(ErrorDuplicate);

            var def = new ReportDefinitionEntity
            {
                Id = Guid.NewGuid(),
                Code = request.Code
            };

            _db.ReportDefinitions.Add(def);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetReportDefinitionDTO>.Ok(new GetReportDefinitionDTO(def.Id, def.Code));
        }

        public async Task<ServiceResult<GetReportDefinitionDTO>> UpdateAsync(Guid id, UpdateReportDefinitionDTO request, CancellationToken cancellationToken = default)
        {
            var def = await _db.ReportDefinitions.FindAsync(new object?[] { id }, cancellationToken);
            if (def is null)
                return ServiceResult<GetReportDefinitionDTO>.Fail(ErrorNotFound);

            var exists = await _db.ReportDefinitions
                .AnyAsync(d => d.Code == request.Code && d.Id != id, cancellationToken);
            if (exists)
                return ServiceResult<GetReportDefinitionDTO>.Fail(ErrorDuplicate);

            def.Code = request.Code;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetReportDefinitionDTO>.Ok(new GetReportDefinitionDTO(def.Id, def.Code));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var def = await _db.ReportDefinitions.FindAsync(new object?[] { id }, cancellationToken);
            if (def is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.ReportDefinitions.Remove(def);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
