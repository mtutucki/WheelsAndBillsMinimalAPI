using WheelsAndBillsAPI.Endpoints.Events.EventPart;
using WheelsAndBillsAPI.Endpoints.Events.EventType;
using WheelsAndBillsAPI.Endpoints.Events.FuelingEvent;
using WheelsAndBillsAPI.Endpoints.Events.Parts;
using WheelsAndBillsAPI.Endpoints.Events.RepairEvents;
using WheelsAndBillsAPI.Endpoints.Events.ServiceEvents;
using WheelsAndBillsAPI.Endpoints.Events.VehicleEvents;
using WheelsAndBillsAPI.Endpoints.Events.Workshops;

namespace WheelsAndBillsAPI.Endpoints.Events
{
    public static class EventsEndpoints
    {
        public static IEndpointRouteBuilder MapEventsEndpoints(this IEndpointRouteBuilder app)
        {
            var eventParts = app
                .MapGroup("/event-parts")
                .WithTags("Event parts")
                .RequireAuthorization();

            var eventType = app
                .MapGroup("/event-types")
                .WithTags("Event types")
                .RequireAuthorization();

            var fuelingEvents = app
                .MapGroup("/event-fuel")
                .WithTags("Fuel events")
                .RequireAuthorization();

            var parts = app
                .MapGroup("/parts")
                .WithTags("Parts")
                .RequireAuthorization();

            var repairEvent = app
                .MapGroup("/event-repair")
                .WithTags("Event repair")
                .RequireAuthorization();

            var serviceEvent = app
                .MapGroup("/event-service")
                .WithTags("Event service")
                .RequireAuthorization();
            
            var vehicleEvent = app
                .MapGroup("/event-vehicle")
                .WithTags("Event vehicle")
                .RequireAuthorization();

            var worskshop = app
                .MapGroup("/workshop")
                .WithTags("Workshop")
                .RequireAuthorization();


            eventParts.MapCreateEventPart();
            eventParts.MapUpdateEventPart();
            eventParts.MapDeleteEventPart();
            eventParts.MapGetEventPartsByRepairEvent();

            eventType.MapCreateEventType();
            eventType.MapUpdateEventType();
            eventType.MapDeleteEventType();
            eventType.MapGetEventTypes();
            eventType.MapGetEventTypeById();

            fuelingEvents.MapCreateFuelingEvent();
            fuelingEvents.MapUpdateFuelingEvent();
            fuelingEvents.MapDeleteFuelingEvent();
            fuelingEvents.MapGetFuelingEvents();
            fuelingEvents.MapGetFuelingEventById();

            parts.MapCreatePart();
            parts.MapUpdatePart();
            parts.MapDeletePart();
            parts.MapGetParts();
            parts.MapGetPartById();

            repairEvent.MapCreateRepairEvent();
            repairEvent.MapUpdateRepairEvent();
            repairEvent.MapDeleteRepairEvent();
            repairEvent.MapGetRepairEvents();
            repairEvent.MapGetRepairEventById();

            serviceEvent.MapCreateServiceEvent();
            serviceEvent.MapUpdateServiceEvent();
            serviceEvent.MapDeleteServiceEvent();
            serviceEvent.MapGetServiceEvents();
            serviceEvent.MapGetServiceEventById();

            vehicleEvent.MapCreateVehicleEvent();
            vehicleEvent.MapUpdateVehicleEvent();
            vehicleEvent.MapDeleteVehicleEvent();
            vehicleEvent.MapGetVehicleEvents();
            vehicleEvent.MapGetVehicleEventById();

            worskshop.MapCreateWorkshop();
            worskshop.MapUpdateWorkshop();
            worskshop.MapDeleteWorkshop();
            worskshop.MapGetWorkshops();
            worskshop.MapGetWorkshopById();

            return app;
        }
    }
}
