using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheelsAndBills.API.Migrations
{
    [Migration("20260210190000_UpdateCostsAndRepairsReportObjects")]
    public partial class UpdateCostsAndRepairsReportObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
CREATE OR ALTER VIEW vw_Report_CostsByEventType_Base AS
    SELECT
        ve.Id AS VehicleEventId,
        ve.VehicleId,
        ve.EventDate,
        et.Name AS EventType,
        CAST('FUEL' AS nvarchar(20)) AS SourceType,
        CAST(f.TotalPrice AS decimal(18,2)) AS Amount,
        CAST(0 AS decimal(18,2)) AS LaborAmount,
        CAST(0 AS decimal(18,2)) AS PartsAmount
    FROM FuelingEvents f
    INNER JOIN VehicleEvents ve ON ve.Id = f.VehicleEventId
    INNER JOIN EventTypes et ON et.Id = ve.EventTypeId

    UNION ALL

    SELECT
        ve.Id AS VehicleEventId,
        ve.VehicleId,
        ve.EventDate,
        et.Name AS EventType,
        CAST('REPAIR' AS nvarchar(20)) AS SourceType,
        CAST(r.LaborCost + ISNULL(parts.PartsCost, 0) AS decimal(18,2)) AS Amount,
        CAST(r.LaborCost AS decimal(18,2)) AS LaborAmount,
        CAST(ISNULL(parts.PartsCost, 0) AS decimal(18,2)) AS PartsAmount
    FROM RepairEvents r
    INNER JOIN VehicleEvents ve ON ve.Id = r.VehicleEventId
    INNER JOIN EventTypes et ON et.Id = ve.EventTypeId
    OUTER APPLY (
        SELECT SUM(ep.Price) AS PartsCost
        FROM EventParts ep
        WHERE ep.RepairEventId = r.Id
    ) parts

    UNION ALL

    SELECT
        ve.Id AS VehicleEventId,
        ve.VehicleId,
        ve.EventDate,
        et.Name AS EventType,
        CAST('COST' AS nvarchar(20)) AS SourceType,
        CAST(c.Amount AS decimal(18,2)) AS Amount,
        CAST(0 AS decimal(18,2)) AS LaborAmount,
        CAST(0 AS decimal(18,2)) AS PartsAmount
    FROM Costs c
    INNER JOIN VehicleEvents ve ON ve.Id = c.VehicleEventId
    INNER JOIN EventTypes et ON et.Id = ve.EventTypeId;

CREATE OR ALTER FUNCTION fn_Report_CostsByEventTypeTotal(
    @VehicleId uniqueidentifier,
    @DateFrom date,
    @DateTo date
)
RETURNS decimal(18,2)
AS
BEGIN
    RETURN (
        SELECT ISNULL(SUM(b.Amount), 0)
        FROM vw_Report_CostsByEventType_Base b
        WHERE b.VehicleId = @VehicleId
          AND b.EventDate >= @DateFrom
          AND b.EventDate <= @DateTo
    );
END;

CREATE OR ALTER VIEW vw_Report_CostsByEventType AS
    SELECT
        b.VehicleId,
        b.EventType,
        CAST(SUM(b.Amount) AS decimal(18,2)) AS TotalAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'FUEL' THEN b.Amount ELSE 0 END) AS decimal(18,2)) AS FuelAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'REPAIR' THEN b.LaborAmount ELSE 0 END) AS decimal(18,2)) AS RepairLaborAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'REPAIR' THEN b.PartsAmount ELSE 0 END) AS decimal(18,2)) AS RepairPartsAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'COST' THEN b.Amount ELSE 0 END) AS decimal(18,2)) AS OtherAmount,
        COUNT(DISTINCT b.VehicleEventId) AS EventsCount
    FROM vw_Report_CostsByEventType_Base b
    GROUP BY b.VehicleId, b.EventType;

CREATE OR ALTER PROCEDURE sp_Report_CostsByEventType
    @VehicleId uniqueidentifier,
    @DateFrom date,
    @DateTo date
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.EventDate,
        b.EventType,
        CAST(SUM(b.Amount) AS decimal(18,2)) AS TotalAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'FUEL' THEN b.Amount ELSE 0 END) AS decimal(18,2)) AS FuelAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'REPAIR' THEN b.LaborAmount ELSE 0 END) AS decimal(18,2)) AS RepairLaborAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'REPAIR' THEN b.PartsAmount ELSE 0 END) AS decimal(18,2)) AS RepairPartsAmount,
        CAST(SUM(CASE WHEN b.SourceType = 'COST' THEN b.Amount ELSE 0 END) AS decimal(18,2)) AS OtherAmount,
        COUNT(DISTINCT b.VehicleEventId) AS EventsCount,
        dbo.fn_Report_CostsByEventTypeTotal(@VehicleId, @DateFrom, @DateTo) AS OverallTotal
    FROM vw_Report_CostsByEventType_Base b
    WHERE b.VehicleId = @VehicleId
      AND b.EventDate >= @DateFrom
      AND b.EventDate <= @DateTo
    GROUP BY b.VehicleEventId, b.EventDate, b.EventType
    ORDER BY b.EventDate, b.EventType;
END;

CREATE OR ALTER VIEW vw_Report_RepairsHistory AS
    SELECT
        ve.VehicleId,
        r.Id AS RepairEventId,
        ve.EventDate,
        ve.Mileage,
        ve.Description,
        ws.Name AS WorkshopName,
        parts.PartsList,
        CAST(r.LaborCost AS decimal(18,2)) AS LaborCost,
        CAST(ISNULL(parts.PartsCost, 0) AS decimal(18,2)) AS PartsCost,
        CAST(r.LaborCost + ISNULL(parts.PartsCost, 0) AS decimal(18,2)) AS TotalCost
    FROM RepairEvents r
    INNER JOIN VehicleEvents ve ON ve.Id = r.VehicleEventId
    LEFT JOIN ServiceEvents se ON se.VehicleEventId = ve.Id
    LEFT JOIN Workshops ws ON ws.Id = se.WorkshopId
    OUTER APPLY (
        SELECT
            SUM(ep.Price) AS PartsCost,
            STRING_AGG(p.Name, ', ') AS PartsList
        FROM EventParts ep
        INNER JOIN Parts p ON p.Id = ep.PartId
        WHERE ep.RepairEventId = r.Id
    ) parts;

CREATE OR ALTER PROCEDURE sp_Report_RepairsHistory
    @VehicleId uniqueidentifier,
    @DateFrom date,
    @DateTo date
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        RepairEventId,
        EventDate,
        Mileage,
        Description,
        WorkshopName,
        PartsList,
        LaborCost,
        PartsCost,
        TotalCost
    FROM vw_Report_RepairsHistory
    WHERE VehicleId = @VehicleId
      AND EventDate >= @DateFrom
      AND EventDate <= @DateTo
    ORDER BY EventDate;
END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
DROP PROCEDURE IF EXISTS sp_Report_CostsByEventType;
DROP FUNCTION IF EXISTS fn_Report_CostsByEventTypeTotal;
DROP VIEW IF EXISTS vw_Report_CostsByEventType;
DROP VIEW IF EXISTS vw_Report_CostsByEventType_Base;

DROP PROCEDURE IF EXISTS sp_Report_RepairsHistory;
DROP VIEW IF EXISTS vw_Report_RepairsHistory;
");
        }
    }
}
