using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.SystemSettings
{
    public static class SystemSettingsEndpoints
    {

        public static RouteHandlerBuilder MapGetSystemSettings(this RouteGroupBuilder app)
        {
            return app.MapGet("/system-settings", async (AppDbContext db) =>
            {
                var settings = await db.SystemSettings
                    .OrderBy(s => s.Key)
                    .Select(s => new GetSystemSettingDTO(
                        s.Id,
                        s.Key,
                        s.Value
                    ))
                    .ToListAsync();

                return Results.Ok(settings);
            });
        }

        public static RouteHandlerBuilder MapGetSystemSettingById(this RouteGroupBuilder app)
        {
            return app.MapGet("/system-settings/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var setting = await db.SystemSettings
                    .Where(s => s.Id == id)
                    .Select(s => new GetSystemSettingDTO(
                        s.Id,
                        s.Key,
                        s.Value
                    ))
                    .FirstOrDefaultAsync();

                return setting is null
                    ? Results.NotFound()
                    : Results.Ok(setting);
            });
        }

        public static RouteHandlerBuilder MapGetSystemSettingByKey(this RouteGroupBuilder app)
        {
            return app.MapGet("/system-settings/key/{key}", async (
                string key,
                AppDbContext db) =>
            {
                var setting = await db.SystemSettings
                    .Where(s => s.Key == key)
                    .Select(s => new GetSystemSettingDTO(
                        s.Id,
                        s.Key,
                        s.Value
                    ))
                    .FirstOrDefaultAsync();

                return setting is null
                    ? Results.NotFound()
                    : Results.Ok(setting);
            });
        }


        public static RouteHandlerBuilder MapCreateSystemSetting(this RouteGroupBuilder app)
        {
            return app.MapPost("/system-settings", async (
                CreateSystemSettingDTO request,
                AppDbContext db) =>
            {
                var exists = await db.SystemSettings
                    .AnyAsync(s => s.Key == request.Key);

                if (exists)
                    return Results.BadRequest("System setting with this key already exists");

                var setting = new Domain.Entities.Admin.SystemSetting
                {
                    Id = Guid.NewGuid(),
                    Key = request.Key,
                    Value = request.Value
                };

                db.SystemSettings.Add(setting);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/system-settings/{setting.Id}",
                    new GetSystemSettingDTO(
                        setting.Id,
                        setting.Key,
                        setting.Value
                    )
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateSystemSetting(this RouteGroupBuilder app)
        {
            return app.MapPut("/system-settings/{id:guid}", async (
                Guid id,
                UpdateSystemSettingDTO request,
                AppDbContext db) =>
            {
                var setting = await db.SystemSettings.FindAsync(id);
                if (setting is null)
                    return Results.NotFound();

                setting.Value = request.Value;
                await db.SaveChangesAsync();

                return Results.Ok(new GetSystemSettingDTO(
                    setting.Id,
                    setting.Key,
                    setting.Value
                ));
            });
        }


        public static RouteHandlerBuilder MapDeleteSystemSetting(this RouteGroupBuilder app)
        {
            return app.MapDelete("/system-settings/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var setting = await db.SystemSettings.FindAsync(id);
                if (setting is null)
                    return Results.NotFound();

                db.SystemSettings.Remove(setting);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
