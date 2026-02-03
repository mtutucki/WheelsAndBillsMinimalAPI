using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Abstractions.Results;
using ReportEntity = WheelsAndBills.Domain.Entities.Report.Report;
using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;

namespace WheelsAndBills.Application.Features.Reports.Reports
{
    public class ReportsService : IReportsService
    {
        private const string ErrorNotFound = "NotFound";
        private const string ErrorUserMissing = "User does not exist";
        private const string ErrorDefinitionMissing = "ReportDefinition does not exist";

        private readonly IAppDbContext _db;

        public ReportsService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<GetReportDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Reports
                .Select(r => new GetReportDTO(
                    r.Id,
                    r.UserId,
                    r.ReportDefinitionId,
                    r.CreatedAt
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<GetReportDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Reports
                .Where(r => r.Id == id)
                .Select(r => new GetReportDTO(
                    r.Id,
                    r.UserId,
                    r.ReportDefinitionId,
                    r.CreatedAt
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ServiceResult<GetReportDTO>> CreateAsync(CreateReportDTO request, CancellationToken cancellationToken = default)
        {
            var userExists = await _db.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
            if (!userExists)
                return ServiceResult<GetReportDTO>.Fail(ErrorUserMissing);

            var definitionExists = await _db.ReportDefinitions
                .AnyAsync(d => d.Id == request.ReportDefinitionId, cancellationToken);
            if (!definitionExists)
                return ServiceResult<GetReportDTO>.Fail(ErrorDefinitionMissing);

            var report = new ReportEntity
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ReportDefinitionId = request.ReportDefinitionId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Reports.Add(report);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetReportDTO>.Ok(new GetReportDTO(
                report.Id,
                report.UserId,
                report.ReportDefinitionId,
                report.CreatedAt
            ));
        }

        public async Task<ServiceResult<GetReportDTO>> UpdateAsync(Guid id, UpdateReportDTO request, CancellationToken cancellationToken = default)
        {
            var report = await _db.Reports.FindAsync(new object?[] { id }, cancellationToken);
            if (report is null)
                return ServiceResult<GetReportDTO>.Fail(ErrorNotFound);

            var definitionExists = await _db.ReportDefinitions
                .AnyAsync(d => d.Id == request.ReportDefinitionId, cancellationToken);
            if (!definitionExists)
                return ServiceResult<GetReportDTO>.Fail(ErrorDefinitionMissing);

            report.ReportDefinitionId = request.ReportDefinitionId;
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult<GetReportDTO>.Ok(new GetReportDTO(
                report.Id,
                report.UserId,
                report.ReportDefinitionId,
                report.CreatedAt
            ));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var report = await _db.Reports.FindAsync(new object?[] { id }, cancellationToken);
            if (report is null)
                return ServiceResult.Fail(ErrorNotFound);

            _db.Reports.Remove(report);
            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
