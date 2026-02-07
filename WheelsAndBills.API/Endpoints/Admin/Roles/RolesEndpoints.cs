using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WheelsAndBills.Domain.Entities.Auth;

namespace WheelsAndBills.API.Endpoints.Admin.Roles
{
    public static class RolesEndpoints
    {
        public record CreateRoleRequest(string Name);
        public record SetUserRoleRequest(string Role);

        public static IEndpointRouteBuilder MapRoleAdminEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("/admin/roles")
                .WithTags("Admin - Roles")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

            group.MapGet("", (RoleManager<IdentityRole<Guid>> roleManager) =>
            {
                var roles = roleManager.Roles.Select(r => new { r.Id, r.Name });
                return Results.Ok(roles);
            });

            group.MapPost("", async (
                CreateRoleRequest request,
                RoleManager<IdentityRole<Guid>> roleManager) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return Results.BadRequest("Role name is required");

                var result = await roleManager.CreateAsync(new IdentityRole<Guid>(request.Name));
                return result.Succeeded
                    ? Results.Ok()
                    : Results.BadRequest(result.Errors);
            });

            group.MapDelete("/{id:guid}", async (
                Guid id,
                RoleManager<IdentityRole<Guid>> roleManager) =>
            {
                var role = await roleManager.FindByIdAsync(id.ToString());
                if (role is null) return Results.NotFound();

                var result = await roleManager.DeleteAsync(role);
                return result.Succeeded
                    ? Results.NoContent()
                    : Results.BadRequest(result.Errors);
            });

            group.MapGet("/users", (UserManager<ApplicationUser> userManager) =>
            {
                var users = userManager.Users.Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.UserName
                });
                return Results.Ok(users);
            });

            group.MapGet("/users/{id:guid}", async (
                Guid id,
                UserManager<ApplicationUser> userManager) =>
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user is null) return Results.NotFound();

                var roles = await userManager.GetRolesAsync(user);
                return Results.Ok(new
                {
                    user.Id,
                    user.Email,
                    user.UserName,
                    Roles = roles
                });
            });

            group.MapPost("/users/{id:guid}/roles", async (
                Guid id,
                SetUserRoleRequest request,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole<Guid>> roleManager) =>
            {
                if (string.IsNullOrWhiteSpace(request.Role))
                    return Results.BadRequest("Role is required");

                var user = await userManager.FindByIdAsync(id.ToString());
                if (user is null) return Results.NotFound();

                if (!await roleManager.RoleExistsAsync(request.Role))
                    return Results.BadRequest("Role does not exist");

                var result = await userManager.AddToRoleAsync(user, request.Role);
                return result.Succeeded
                    ? Results.Ok()
                    : Results.BadRequest(result.Errors);
            });

            group.MapDelete("/users/{id:guid}/roles/{role}", async (
                Guid id,
                string role,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole<Guid>> roleManager) =>
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user is null) return Results.NotFound();

                if (!await roleManager.RoleExistsAsync(role))
                    return Results.BadRequest("Role does not exist");

                var result = await userManager.RemoveFromRoleAsync(user, role);
                return result.Succeeded
                    ? Results.Ok()
                    : Results.BadRequest(result.Errors);
            });

            return app;
        }
    }
}
