using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheelsAndBills.API.Migrations
{
    [Migration("20260210160000_UpdateMonthlyCostsReportObjects")]
    public partial class UpdateMonthlyCostsReportObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
CREATE OR ALTER VIEW vw_Report_MonthlyCosts_Base AS
    SELECT
        ve.VehicleId,
        ve.EventDate,
        CAST('FUEL' AS nvarchar(20)) AS SourceType,
        CAST(f.TotalPrice AS decimal(18,2)) AS Amount,
        CAST(0 AS decimal(18,2)) AS LaborAmount,
        CAST(0 AS decimal(18,2)) AS PartsAmount
    FROM FuelingEvents f
    INNER JOIN VehicleEvents ve ON ve.Id = f.VehicleEventId

    UNION ALL

    SELECT
        ve.VehicleId,
        ve.EventDate,
        CAST('REPAIR' AS nvarchar(20)) AS SourceType,
        CAST(r.LaborCost + ISNULL(parts.PartsCost, 0) AS decimal(18,2)) AS Amount,
        CAST(r.LaborCost AS decimal(18,2)) AS LaborAmount,
        CAST(ISNULL(parts.PartsCost, 0) AS decimal(18,2)) AS PartsAmount
    FROM RepairEvents r
    INNER JOIN VehicleEvents ve ON ve.Id = r.VehicleEventId
    OUTER APPLY (
        SELECT SUM(ep.Price) AS PartsCost
        FROM EventParts ep
        WHERE ep.RepairEventId = r.Id
    ) parts

    UNION ALL

    SELECT
        ve.VehicleId,
        ve.EventDate,
        CAST('COST' AS nvarchar(20)) AS SourceType,
        CAST(c.Amount AS decimal(18,2)) AS Amount,
        CAST(0 AS decimal(18,2)) AS LaborAmount,
        CAST(0 AS decimal(18,2)) AS PartsAmount
    FROM Costs c
    INNER JOIN VehicleEvents ve ON ve.Id = c.VehicleEventId;

CREATE OR ALTER FUNCTION fn_Report_MonthlyCostTotal(
    @VehicleId uniqueidentifier,
    @DateFrom date,
    @DateTo date
)
RETURNS decimal(18,2)
AS
BEGIN
    RETURN (
        SELECT ISNULL(SUM(b.Amount), 0)
        FROM vw_Report_MonthlyCosts_Base b
        WHERE b.VehicleId = @VehicleId
          AND b.EventDate >= @DateFrom
          AND b.EventDate <= @DateTo
    );
END;

CREATE OR ALTER VIEW vw_Report_MonthlyCosts AS
    SELECT
        b.VehicleId,
        YEAR(b.EventDate) AS [Year],
        MONTH(b.EventDate) AS [Month],
        CAST(SUM(b.Amount) AS decimal(18,2)) AS TotalAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'FUEL' THEN b.Amount ELSE 0 END) AS decimal(18,2)) AS FuelAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'REPAIR' THEN b.LaborAmount ELSE 0 END) AS decimal(18,2)) AS RepairLaborAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'REPAIR' THEN b.PartsAmount ELSE 0 END) AS decimal(18,2)) AS RepairPartsAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'COST' THEN b.Amount ELSE 0 END) AS decimal(18,2)) AS OtherAmount,
        COUNT(*) AS EventsCount
    FROM vw_Report_MonthlyCosts_Base b
    GROUP BY b.VehicleId, YEAR(b.EventDate), MONTH(b.EventDate);

CREATE OR ALTER PROCEDURE sp_Report_MonthlyCosts
    @VehicleId uniqueidentifier,
    @DateFrom date,
    @DateTo date
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Year],
        [Month],
        TotalAmount,
        FuelAmount,
        RepairLaborAmount,
        RepairPartsAmount,
        OtherAmount,
        EventsCount,
        dbo.fn_Report_MonthlyCostTotal(@VehicleId, @DateFrom, @DateTo) AS OverallTotal
    FROM vw_Report_MonthlyCosts
    WHERE VehicleId = @VehicleId
      AND DATEFROMPARTS([Year], [Month], 1) >= DATEFROMPARTS(YEAR(@DateFrom), MONTH(@DateFrom), 1)
      AND DATEFROMPARTS([Year], [Month], 1) <= DATEFROMPARTS(YEAR(@DateTo), MONTH(@DateTo), 1)
    ORDER BY [Year], [Month];
END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
DROP PROCEDURE IF EXISTS sp_Report_MonthlyCosts;
DROP FUNCTION IF EXISTS fn_Report_MonthlyCostTotal;
DROP VIEW IF EXISTS vw_Report_MonthlyCosts;
DROP VIEW IF EXISTS vw_Report_MonthlyCosts_Base;
");
        }
    }
}
