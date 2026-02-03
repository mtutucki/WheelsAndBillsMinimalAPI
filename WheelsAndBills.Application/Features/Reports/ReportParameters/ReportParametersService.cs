using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using ReportParameterEntity = WheelsAndBills.Domain.Entities.Report.ReportParameter;
using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;

namespace WheelsAndBills.Application.Features.Reports.ReportParameters
{
    public class ReportParametersService : IReportParametersService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorReportMissing = "Report does not exist";

        private readonly IAppDbContext _db;

        public ReportParametersService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetReportParameterDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.ReportParameters
                .Select(p => new GetReportParameterDTO(
                    p.Id,
                    p.ReportId,
                    p.Name,
                    p.Value
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetReportParameterDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.ReportParameters
                .Where(p => p.Id == id)
                .Select(p => new GetReportParameterDTO(
                    p.Id,
                    p.ReportId,
                    p.Name,
                    p.Value
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetReportParameterDTO>> CreateAsync(CreateReportParameterDTO request, CancellationToken cancellationToken = default)
        {
            var reportExists = await _db.Reports
                .AnyAsync(r => r.Id == request.ReportId, cancellationToken);
            if (!reportExists)
                return ServiceResult<GetReportParameterDTO>.Fail(ErrorReportMissing);

            var param = new ReportParameterEntity
            {
                Id = Guid.NewGuid(),
                ReportId = request.ReportId,
                Name = request.Name,
                Value = request.Value
            };

            _db.ReportParameters.Add(param);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetReportParameterDTO>.Ok(new GetReportParameterDTO(
                param.Id,
                param.ReportId,
                param.Name,
                param.Value
            ));
        }

        public async Task<ServiceResult<GetReportParameterDTO>> UpdateAsync(Guid id, UpdateReportParameterDTO request, CancellationToken cancellationToken = default)
        {
            var param = await _db.ReportParameters.FindAsync(new object?[] { id }, cancellationToken);
            if (param is null)
                return ServiceResult<GetReportParameterDTO>.Fail(ErrorNotFound);

            param.Name = request.Name;
            param.Value = request.Value;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetReportParameterDTO>.Ok(new GetReportParameterDTO(
                param.Id,
                param.ReportId,
                param.Name,
                param.Value
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var param = await _db.ReportParameters.FindAsync(new object?[] { id }, cancellationToken);
            if (param is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.ReportParameters.Remove(param);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
