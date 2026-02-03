using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using GeneratedReportEntity = WheelsAndBills.Domain.Entities.Report.GeneratedReport;
using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;

namespace WheelsAndBills.Application.Features.Reports.GeneratedReports
{
    public class GeneratedReportsService : IGeneratedReportsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorReportMissing = "Report does not exist";

        private readonly IAppDbContext _db;

        public GeneratedReportsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetGeneratedReportDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.GeneratedReports
                .Select(r => new GetGeneratedReportDTO(
                    r.Id,
                    r.ReportId,
                    r.FilePath
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetGeneratedReportDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.GeneratedReports
                .Where(r => r.Id == id)
                .Select(r => new GetGeneratedReportDTO(
                    r.Id,
                    r.ReportId,
                    r.FilePath
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetGeneratedReportDTO>> CreateAsync(CreateGeneratedReportDTO request, CancellationToken cancellationToken = default)
        {
            var reportExists = await _db.Reports
                .AnyAsync(r => r.Id == request.ReportId, cancellationToken);
            if (!reportExists)
                return ServiceResult<GetGeneratedReportDTO>.Fail(ErrorReportMissing);

            var generated = new GeneratedReportEntity
            {
                Id = Guid.NewGuid(),
                ReportId = request.ReportId,
                FilePath = request.FilePath
            };

            _db.GeneratedReports.Add(generated);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetGeneratedReportDTO>.Ok(new GetGeneratedReportDTO(
                generated.Id,
                generated.ReportId,
                generated.FilePath
            ));
        }

        public async Task<ServiceResult<GetGeneratedReportDTO>> UpdateAsync(Guid id, UpdateGeneratedReportDTO request, CancellationToken cancellationToken = default)
        {
            var generated = await _db.GeneratedReports.FindAsync(new object?[] { id }, cancellationToken);
            if (generated is null)
                return ServiceResult<GetGeneratedReportDTO>.Fail(ErrorNotFound);

            generated.FilePath = request.FilePath;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetGeneratedReportDTO>.Ok(new GetGeneratedReportDTO(
                generated.Id,
                generated.ReportId,
                generated.FilePath
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var generated = await _db.GeneratedReports.FindAsync(new object?[] { id }, cancellationToken);
            if (generated is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.GeneratedReports.Remove(generated);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
