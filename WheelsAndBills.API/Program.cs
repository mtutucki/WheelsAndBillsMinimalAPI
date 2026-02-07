using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WheelsAndBills.Domain.Entities.Auth;
using WheelsAndBills.API.Endpoints.Auth;
using WheelsAndBills.Infrastructure.Persistence;
using WheelsAndBills.API.Endpoints.Vehicles;
using WheelsAndBills.API.Endpoints.Account;
using WheelsAndBills.API.Endpoints.Admin;
using WheelsAndBills.API.Endpoints.Cost;
using WheelsAndBills.API.Endpoints.Events;
using WheelsAndBills.API.Endpoints.Notifications;
using WheelsAndBills.API.Endpoints.Reports;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.Features.Vehicles.UserVehicles;
using WheelsAndBills.Application.Features.Vehicles.VehicleBrands;
using WheelsAndBills.Application.Features.Vehicles.VehicleMileage;
using WheelsAndBills.Application.Features.Vehicles.VehicleModels;
using WheelsAndBills.Application.Features.Vehicles.VehicleNotes;
using WheelsAndBills.Application.Features.Vehicles.VehicleStatuses;
using WheelsAndBills.Application.Features.Vehicles.VehicleTypes;
using WheelsAndBills.Application.Features.Vehicles.VehiclesAdmin;
using WheelsAndBills.Application.Features.Events.EventParts;
using WheelsAndBills.Application.Features.Events.EventTypes;
using WheelsAndBills.Application.Features.Events.FuelingEvents;
using WheelsAndBills.Application.Features.Events.Parts;
using WheelsAndBills.Application.Features.Events.RepairEvents;
using WheelsAndBills.Application.Features.Events.ServiceEvents;
using WheelsAndBills.Application.Features.Events.VehicleEvents;
using WheelsAndBills.Application.Features.Events.Workshops;
using WheelsAndBills.Application.Features.Auth;
using WheelsAndBills.Application.Features.Account;
using WheelsAndBills.Application.Features.Cost.Costs;
using WheelsAndBills.Application.Features.Cost.CostTypes;
using WheelsAndBills.Application.Features.Cost.RecurringCosts;
using WheelsAndBills.Application.Features.Notifications.NotificationTypes;
using WheelsAndBills.Application.Features.Notifications.Notifications;
using WheelsAndBills.Application.Features.Reports.GeneratedReports;
using WheelsAndBills.Application.Features.Reports.ReportDefinitions;
using WheelsAndBills.Application.Features.Reports.ReportParameters;
using WheelsAndBills.Application.Features.Reports.Reports;
using WheelsAndBills.Application.Features.Reports.ReportQueries;
using WheelsAndBills.Application.Features.Admin.SystemSettings;
using WheelsAndBills.Application.Features.Admin.Dictionaries;
using WheelsAndBills.Application.Features.Admin.DictionaryItems;
using WheelsAndBills.Application.Features.Admin.FileResources;
using WheelsAndBills.Application.Features.Admin.ContentBlocks;
using WheelsAndBills.Application.Features.Admin.ContentPages;
using WheelsAndBills.Application.Features.Dashboard;
using WheelsAndBills.API.Endpoints.Dashboard;
using WheelsAndBills.API.Endpoints.Reports.ReportQueries;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Wpisz: Bearer {twój JWT token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        sql => sql.MigrationsAssembly(typeof(Program).Assembly.FullName)));

builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IUserVehiclesService, UserVehiclesService>();
builder.Services.AddScoped<IVehicleTypesService, VehicleTypesService>();
builder.Services.AddScoped<IVehicleBrandsService, VehicleBrandsService>();
builder.Services.AddScoped<IVehicleModelsService, VehicleModelsService>();
builder.Services.AddScoped<IVehicleStatusesService, VehicleStatusesService>();
builder.Services.AddScoped<IVehicleMileageService, VehicleMileageService>();
builder.Services.AddScoped<IVehicleNotesService, VehicleNotesService>();
builder.Services.AddScoped<IVehiclesAdminService, VehiclesAdminService>();
builder.Services.AddScoped<IEventTypesService, EventTypesService>();
builder.Services.AddScoped<IWorkshopsService, WorkshopsService>();
builder.Services.AddScoped<IPartsService, PartsService>();
builder.Services.AddScoped<IEventPartsService, EventPartsService>();
builder.Services.AddScoped<IFuelingEventsService, FuelingEventsService>();
builder.Services.AddScoped<IRepairEventsService, RepairEventsService>();
builder.Services.AddScoped<IServiceEventsService, ServiceEventsService>();
builder.Services.AddScoped<IVehicleEventsService, VehicleEventsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICostTypesService, CostTypesService>();
builder.Services.AddScoped<ICostsService, CostsService>();
builder.Services.AddScoped<IRecurringCostsService, RecurringCostsService>();
builder.Services.AddScoped<INotificationTypesService, NotificationTypesService>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();
builder.Services.AddScoped<IReportDefinitionsService, ReportDefinitionsService>();
builder.Services.AddScoped<IReportParametersService, ReportParametersService>();
builder.Services.AddScoped<IGeneratedReportsService, GeneratedReportsService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<IReportQueriesService, ReportQueriesService>();
builder.Services.AddScoped<ISystemSettingsService, SystemSettingsService>();
builder.Services.AddScoped<IDictionariesService, DictionariesService>();
builder.Services.AddScoped<IDictionaryItemsService, DictionaryItemsService>();
builder.Services.AddScoped<IFileResourcesService, FileResourcesService>();
builder.Services.AddScoped<IContentBlocksService, ContentBlocksService>();
builder.Services.AddScoped<IContentPagesService, ContentPagesService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
        )
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

await SeedRolesAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("DevCors");
app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("/api");

api.MapAuthEndpoints();
api.MapAccountEndpoints();
api.MapVehiclesEndpoints();
api.MapAdminEndpoints();
api.MapCostsEndpoints();
api.MapEventsEndpoints();
api.MapNotificationsEndpoints();
api.MapReportEndpoints();
api.MapDashboardEndpoints();
app.MapReportGeneration();

app.Run();

static async Task SeedRolesAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    var roles = new[] { "Admin", "User", "Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
    }
}
