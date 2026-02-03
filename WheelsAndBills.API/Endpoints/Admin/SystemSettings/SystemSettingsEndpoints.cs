using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.SystemSettings;

namespace WheelsAndBills.API.Endpoints.Admin.SystemSettings
{
    public static class SystemSettingsEndpoints
    {

        public static RouteHandlerBuilder MapGetSystemSettings(this RouteGroupBuilder app)
        {
            return app.MapGet("/system-settings", async (
                ISystemSettingsService systemSettings,
                CancellationToken cancellationToken) =>
            {
                var settings = await systemSettings.GetAllAsync(cancellationToken);

                return Results.Ok(settings);
            });
        }

        public static RouteHandlerBuilder MapGetSystemSettingById(this RouteGroupBuilder app)
        {
            return app.MapGet("/system-settings/{id:guid}", async (
                Guid id,
                ISystemSettingsService systemSettings,
                CancellationToken cancellationToken) =>
            {
                var setting = await systemSettings.GetByIdAsync(id, cancellationToken);

                return setting is null
                    ? Results.NotFound()
                    : Results.Ok(setting);
            });
        }

        public static RouteHandlerBuilder MapGetSystemSettingByKey(this RouteGroupBuilder app)
        {
            return app.MapGet("/system-settings/key/{key}", async (
                string key,
                ISystemSettingsService systemSettings,
                CancellationToken cancellationToken) =>
            {
                var setting = await systemSettings.GetByKeyAsync(key, cancellationToken);

                return setting is null
                    ? Results.NotFound()
                    : Results.Ok(setting);
            });
        }


        public static RouteHandlerBuilder MapCreateSystemSetting(this RouteGroupBuilder app)
        {
            return app.MapPost("/system-settings", async (
                CreateSystemSettingDTO request,
                ISystemSettingsService systemSettings,
                CancellationToken cancellationToken) =>
            {
                var result = await systemSettings.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/system-settings/{result.Data!.Id}",
                    result.Data
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateSystemSetting(this RouteGroupBuilder app)
        {
            return app.MapPut("/system-settings/{id:guid}", async (
                Guid id,
                UpdateSystemSettingDTO request,
                ISystemSettingsService systemSettings,
                CancellationToken cancellationToken) =>
            {
                var result = await systemSettings.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.Ok(result.Data);
            });
        }


        public static RouteHandlerBuilder MapDeleteSystemSetting(this RouteGroupBuilder app)
        {
            return app.MapDelete("/system-settings/{id:guid}", async (
                Guid id,
                ISystemSettingsService systemSettings,
                CancellationToken cancellationToken) =>
            {
                var result = await systemSettings.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
