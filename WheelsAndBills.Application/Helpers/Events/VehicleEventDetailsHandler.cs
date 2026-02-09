using WheelsAndBills.Domain.Entities.Events;
using WheelsAndBills.Domain.Entities.Vehicles;
using WheelsAndBills.Application.DTOs.Events;
using WheelsAndBills.Application.Abstractions.Persistence;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using CostEntity = WheelsAndBills.Domain.Entities.Cost.Cost;
using CostTypeEntity = WheelsAndBills.Domain.Entities.Cost.CostType;

namespace WheelsAndBills.Application.Helpers.Events
{
    public class VehicleEventDetailsHandler
    {
        private const string DefaultCostTypeName = "GENERAL";

        public static async Task Handle(
        CreateMyVehicleEventDTO request,
        Guid vehicleEventId,
        IAppDbContext db)
        {
            if (request.EventTypeId == Guid.Parse("D048C102-BD39-4280-9755-7DA64E418AFF"))
            {
                await HandleFuelingAsync(request, vehicleEventId, db);
            }

            if (request.EventTypeId == Guid.Parse("8FD1DFE4-E0DE-43C1-A74D-DD2E0ECC6D07") || request.EventTypeId == Guid.Parse("FAC15ACE-3DC4-4E64-A515-A7A323622A9F"))
            {
                await HandleServiceAsync(request, vehicleEventId, db);
            }

            if (request.EventTypeId == Guid.Parse("31517083-E941-4C82-A86A-89197BB569B9") || request.EventTypeId == Guid.Parse("03C6CDF1-6C5A-4DDB-8C41-402DE7B5B969"))
            {
                await HandleVehicleStatus(request, db);
            }
        }

        private static async Task HandleFuelingAsync(
            CreateMyVehicleEventDTO request,
            Guid vehicleEventId,
            IAppDbContext db)
        {
            if (request.Data is null)
                throw new InvalidOperationException("Fueling data missing");

            if (!request.Data.TryGetValue("liters", out var litersEl) ||
                !request.Data.TryGetValue("totalCost", out var priceEl))
                throw new InvalidOperationException("Invalid fueling payload");

            var liters = GetDecimal.GetDecimalByJsonElement(litersEl);
            var totalPrice = GetDecimal.GetDecimalByJsonElement(priceEl);

            db.FuelingEvents.Add(new FuelingEvent
            {
                Id = Guid.NewGuid(),
                VehicleEventId = vehicleEventId,
                Liters = liters,
                TotalPrice = totalPrice
            });

            await UpsertCostAsync(db, vehicleEventId, totalPrice);
        }

        private static async Task HandleServiceAsync(
            CreateMyVehicleEventDTO request,
            Guid vehicleEventId,
            IAppDbContext db)
        {
            if (request.Data is null)
                throw new InvalidOperationException("Service data missing");

            ServiceEvent? serviceEvent = null;

            if (request.Data.TryGetValue("workshopId", out var workshopEl))
            {
                var workshopId = Guid.Parse(workshopEl.GetString()!);

                serviceEvent = new ServiceEvent
                {
                    Id = Guid.NewGuid(),
                    VehicleEventId = vehicleEventId,
                    WorkshopId = workshopId
                };

                db.ServiceEvents.Add(serviceEvent);
            }

            RepairEvent? repairEvent = null;
            decimal laborCost = 0;
            decimal partsSum = 0;

            if (request.Data.TryGetValue("laborCost", out var laborEl) ||
                request.Data.TryGetValue("parts", out _))
            {
                laborCost = request.Data.TryGetValue("laborCost", out laborEl)
                    ? GetDecimal.GetDecimalByJsonElement(laborEl)
                    : 0;

                repairEvent = new RepairEvent
                {
                    Id = Guid.NewGuid(),
                    VehicleEventId = vehicleEventId,
                    LaborCost = laborCost
                };

                db.RepairEvents.Add(repairEvent);
            }

            if (request.Data.TryGetValue("parts", out var partsEl) &&
                partsEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var partEl in partsEl.EnumerateArray())
                {
                    var partId = Guid.Parse(partEl.GetProperty("partId").GetString()!);
                    var price = GetDecimal.GetDecimalByJsonElement(
                        partEl.GetProperty("price")
                    );

                    db.EventParts.Add(new EventPart
                    {
                        RepairEventId = repairEvent.Id,
                        PartId = partId,
                        Price = price
                    });

                    partsSum += price;
                }
            }

            var total = laborCost + partsSum;
            if (total > 0)
                await UpsertCostAsync(db, vehicleEventId, total);
        }


        private static async Task HandleVehicleStatus(
            CreateMyVehicleEventDTO request,
            IAppDbContext db)
        {
            if (request.Data is null)
                throw new InvalidOperationException("Status data missing");

            if (!request.Data.TryGetValue("vehicleStatusId", out var statusEl))
                return;

            var statusId = Guid.Parse(statusEl.GetString()!);

            var vehicle = await db.Vehicles
                .FirstOrDefaultAsync(v => v.Id == request.VehicleId);

            if (vehicle is null)
                throw new InvalidOperationException("Vehicle not found");

            var statusExists = await db.VehicleStatuses
                .AnyAsync(s => s.Id == statusId);

            if (!statusExists)
                throw new InvalidOperationException("VehicleStatus does not exist");

            vehicle.StatusId = statusId;
        }

        private static async Task UpsertCostAsync(
            IAppDbContext db,
            Guid vehicleEventId,
            decimal amount)
        {
            var cost = await db.Costs.FirstOrDefaultAsync(c => c.VehicleEventId == vehicleEventId);
            if (cost is null)
            {
                var costTypeId = await GetDefaultCostTypeIdAsync(db);
                cost = new CostEntity
                {
                    Id = Guid.NewGuid(),
                    VehicleEventId = vehicleEventId,
                    CostTypeId = costTypeId,
                    Amount = amount
                };
                db.Costs.Add(cost);
            }
            else
            {
                cost.Amount = amount;
            }
        }

        private static async Task<Guid> GetDefaultCostTypeIdAsync(IAppDbContext db)
        {
            var costType = await db.CostTypes.FirstOrDefaultAsync(c => c.Name == DefaultCostTypeName);
            if (costType is not null) return costType.Id;

            costType = new CostTypeEntity
            {
                Id = Guid.NewGuid(),
                Name = DefaultCostTypeName
            };

            db.CostTypes.Add(costType);
            await db.SaveChangesAsync();
            return costType.Id;
        }
    }
}
