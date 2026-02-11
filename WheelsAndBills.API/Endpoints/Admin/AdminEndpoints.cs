using WheelsAndBills.API.Endpoints.Admin.ContentBlocks;
using WheelsAndBills.API.Endpoints.Admin.ContentPages;
using WheelsAndBills.API.Endpoints.Admin.DicionaryItems;
using WheelsAndBills.API.Endpoints.Admin.Dictionaries;
using WheelsAndBills.API.Endpoints.Admin.FileResources;
using WheelsAndBills.API.Endpoints.Admin.Roles;
using WheelsAndBills.API.Endpoints.Admin.SystemSettings;
using WheelsAndBills.API.Endpoints.Admin.Logs;
using Microsoft.AspNetCore.Authorization;

namespace WheelsAndBills.API.Endpoints.Admin
{
    public static class AdminEndpoints
    {
        public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
        {
            string mapGroup = "/admin";

            var pagesInfo = app
                .MapGroup(mapGroup)
                .WithTags("Frontend - Pages info");

            var contectBlocksAuth = app
                .MapGroup(mapGroup)
                .WithTags("Admin - Content blocks")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Manager" });

            var contentPageAuth = app
                .MapGroup(mapGroup)
                .WithTags("Admin - Content page")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Manager" });

            var dictionary = app
                .MapGroup(mapGroup)
                .WithTags("Admin - Dictionary")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Manager" });

            var dictionaryItems = app
                .MapGroup(mapGroup)
                .WithTags("Admin - Dictionary Items")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Manager" });

            var fileResource = app
                .MapGroup(mapGroup)
                .WithTags("Admin - File resource")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Manager" });

            var systemSettings = app
                .MapGroup(mapGroup)
                .WithTags("Admin - System settings")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Manager" });

            var logs = app
                .MapGroup(mapGroup)
                .WithTags("Admin - Logs")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

            contectBlocksAuth.MapGetContentBlocks();
            contectBlocksAuth.MapCreateContentBlocks();
            contectBlocksAuth.MapUpdateContentBlocks();
            contectBlocksAuth.MapDeleteContentBlocks();

            contentPageAuth.MapGetContentPage();
            contentPageAuth.MapCreateContentPage();
            contentPageAuth.MapDeleteContentPage();
            contentPageAuth.MapUpdateContentPage();

            dictionary.MapCreateDictionary();
            dictionary.MapUpdateDictionary();
            dictionary.MapDeleteDictionary();
            dictionary.MapGetDictionaries();
            dictionary.MapGetDictionaryById();

            dictionaryItems.MapCreateDictionaryItem();
            dictionaryItems.MapUpdateDictionaryItem();
            dictionaryItems.MapDeleteDictionaryItem();
            dictionaryItems.MapGetDictionaryItemsByDictionaryId();
            dictionaryItems.MapGetDictionaryItems();

            fileResource.MapCreateFileResource();
            fileResource.MapUpdateFileResource();
            fileResource.MapDeleteFileResource();
            fileResource.MapGetFileResources();
            fileResource.MapGetFileResourceById();

            systemSettings.MapCreateSystemSetting();
            systemSettings.MapUpdateSystemSetting();
            systemSettings.MapDeleteSystemSetting();
            systemSettings.MapGetSystemSettings();
            systemSettings.MapGetSystemSettingById();
            systemSettings.MapGetSystemSettingByKey();

            logs.MapGetAuditLogs();
            logs.MapGetErrorLogs();

            app.MapRoleAdminEndpoints();

            pagesInfo.MapGetContentBlocksByContentPageId();
            pagesInfo.MapGetContentPageById();
            pagesInfo.MapGetContentPageBySlug();


            return app;
        }
    }
}
