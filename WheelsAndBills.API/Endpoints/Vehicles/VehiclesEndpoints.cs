using WheelsAndBills.API.Endpoints.Events.VehicleEvents;
using WheelsAndBills.API.Endpoints.Vehicles.VehicleBrand;
using WheelsAndBills.API.Endpoints.Vehicles.VehicleMileage;
using WheelsAndBills.API.Endpoints.Vehicles.VehicleModel;
using WheelsAndBills.API.Endpoints.Vehicles.VehicleNote;
using WheelsAndBills.API.Endpoints.Vehicles.Vehicles;
using WheelsAndBills.API.Endpoints.Vehicles.Vehicles.UserVehicles;
using WheelsAndBills.API.Endpoints.Vehicles.VehicleStatus;
using WheelsAndBills.API.Endpoints.Vehicles.VehicleType;
using WheelsAndBills.Application.DTOs.Vehicles;

namespace WheelsAndBills.API.Endpoints.Vehicles
{
    public static class VehiclesEndpoints
    {
        public static IEndpointRouteBuilder MapVehiclesEndpoints(this IEndpointRouteBuilder app)
        {
            var vehicles = app
                .MapGroup("/vehicles")
                .WithTags("Vehicles")
                .RequireAuthorization();

            var userVehicles = app
                .MapGroup("/vehicles/user")
                .WithTags("User vehicles")
                .RequireAuthorization();

            var vehiclesBrand = app
                .MapGroup("/vehicles-brands")
                .WithTags("Vehicles brands")
                .RequireAuthorization();
            
            var vehiclesMileage = app
                .MapGroup("/vehicles-mileage")
                .WithTags("Vehicles mileage")
                .RequireAuthorization();

            var vehiclesModel = app
                .MapGroup("/vehicles-models")
                .WithTags("Vehicles models")
                .RequireAuthorization();

            var vehiclesNotes = app
                .MapGroup("/vehicles-notes")
                .WithTags("Vehicles notes")
                .RequireAuthorization();

            var vehiclesStatus = app
                .MapGroup("/vehicles-status")
                .WithTags("Vehicles status")
                .RequireAuthorization();

            var vehiclesTypes = app
                .MapGroup("/vehicles-types")
                .WithTags("Vehicles types")
                .RequireAuthorization();


            vehicles.MapCreateVehicle();
            vehicles.MapUpdateVehicle();
            vehicles.MapDeleteVehicle();
            vehicles.MapGetVehicles();
            vehicles.MapGetVehicleById();

            userVehicles.MapGetUserVehicles();
            userVehicles.MapGetUserVehicleById();
            userVehicles.MapCreateMyVehicle();
            userVehicles.MapCreateMyVehicleNote();
            userVehicles.MapCreateMyVehicleEvent();
            userVehicles.MapCreateMyVehicleMileage();
            userVehicles.MapDeleteMyVehicleMileage();
            userVehicles.MapDeleteMyVehicleNote();
            userVehicles.MapDeleteMyVehicle();

            vehiclesBrand.MapCreateVehicleBrand();
            vehiclesBrand.MapUpdateVehicleBrand();
            vehiclesBrand.MapDeleteVehicleBrand();
            vehiclesBrand.MapGetVehicleBrands();
            vehiclesBrand.MapGetVehicleBrandById();

            vehiclesMileage.MapCreateVehicleMileage();
            vehiclesMileage.MapUpdateVehicleMileage();
            vehiclesMileage.MapDeleteVehicleMileage();
            vehiclesMileage.MapGetVehicleMileages();
            vehiclesMileage.MapGetVehicleMileageById();

            vehiclesModel.MapCreateVehicleModel();
            vehiclesModel.MapUpdateVehicleModel();
            vehiclesModel.MapDeleteVehicleModel();
            vehiclesModel.MapGetVehicleModels();
            vehiclesModel.MapGetVehicleModelById();

            vehiclesNotes.MapCreateVehicleNote();
            vehiclesNotes.MapUpdateVehicleNote();
            vehiclesNotes.MapDeleteVehicleNote();
            vehiclesNotes.MapGetVehicleNotes();
            vehiclesNotes.MapGetVehicleNoteById();

            vehiclesStatus.MapCreateVehicleStatus();
            vehiclesStatus.MapUpdateVehicleStatus();
            vehiclesStatus.MapDeleteVehicleStatus();
            vehiclesStatus.MapGetVehicleStatuses();
            vehiclesStatus.MapGetVehicleStatusById();

            vehiclesTypes.MapCreateVehicleType();
            vehiclesTypes.MapUpdateVehicleType();
            vehiclesTypes.MapDeleteVehicleType();
            vehiclesTypes.MapGetVehicleTypes();
            vehiclesTypes.MapGetVehicleTypeById();


            return app;
        }
    }
}
