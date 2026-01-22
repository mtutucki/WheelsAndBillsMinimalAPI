using WheelsAndBillsAPI.Endpoints.Admin.ContentBlocks;
using WheelsAndBillsAPI.Endpoints.Admin.ContentPages;
using WheelsAndBillsAPI.Endpoints.Admin.DicionaryItems;
using WheelsAndBillsAPI.Endpoints.Admin.Dictionaries;
using WheelsAndBillsAPI.Endpoints.Admin.FileResources;
using WheelsAndBillsAPI.Endpoints.Admin.SystemSettings;

namespace WheelsAndBillsAPI.Endpoints.Admin
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


            pagesInfo.MapGetContentBlocksByContentPageId();
            pagesInfo.MapGetContentPageById();
            pagesInfo.MapGetContentPageBySlug();


            return app;
        }
    }
}
