using WheelsAndBills.API.Endpoints.Admin.ContentBlocks;
using WheelsAndBills.API.Endpoints.Admin.ContentPages;
using WheelsAndBills.API.Endpoints.Admin.DicionaryItems;
using WheelsAndBills.API.Endpoints.Admin.Dictionaries;
using WheelsAndBills.API.Endpoints.Admin.FileResources;
using WheelsAndBills.API.Endpoints.Admin.Roles;
using WheelsAndBills.API.Endpoints.Admin.SystemSettings;

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
                .RequireAuthorization();

            var contentPageAuth = app
                .MapGroup(mapGroup)
                .WithTags("Admin - Content page")
                .RequireAuthorization();

            var dictionary = app
                .MapGroup(mapGroup)
                .WithTags("Admin - Dictionary")
                .RequireAuthorization();

            var dictionaryItems = app
                .MapGroup(mapGroup)
                .WithTags("Admin - Dictionary Items")
                .RequireAuthorization();

            var fileResource = app
                .MapGroup(mapGroup)
                .WithTags("Admin - File resource")
                .RequireAuthorization();

            var systemSettings = app
                .MapGroup(mapGroup)
                .WithTags("Admin - System settings")
                .RequireAuthorization();

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

            app.MapRoleAdminEndpoints();

            pagesInfo.MapGetContentBlocksByContentPageId();
            pagesInfo.MapGetContentPageById();
            pagesInfo.MapGetContentPageBySlug();


            return app;
        }
    }
}
