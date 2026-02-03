namespace WheelsAndBills.Application.DTOs.Admin.DTO
{
    public record ContentPagePublicDto(
    Guid Id,
    string Title,
    string Slug,
    IEnumerable<ContentBlockPublicDto> Blocks
);

    public record ContentBlockPublicDto(
        string Slot,
        string Content
    );


    public record CreatePageDTO(string Title, string Slug);

    public record GetContentBlockDTO(
         Guid ID,
         Guid ContentPageId ,
         string Content ,
         string Slot
    );

    public record PostContentBlockDTO(
         Guid ContentPageId,
         string Content,
         string Slot
    );




    public record CreateContentPageDTO(
        string Title,
        string Slug
    );

    public record UpdateContentPageDTO(
        string Title,
        string Slug
    );

    public record GetContentPageDTO(
        Guid Id,
        string Title,
        string Slug
    );

    public record CreateDictionaryDTO(string Code);

    public record UpdateDictionaryDTO(string Code);

    public record GetDictionaryDTO(Guid Id, string Code);


    public record CreateDictionaryItemDTO(
        Guid DictionaryId,
        string Value
    );

    public record UpdateDictionaryItemDTO(
        string Value
    );

    public record GetDictionaryItemDTO(
        Guid Id,
        Guid DictionaryId,
        string Value
    );



    public record CreateFileResourceDTO(
        string FileName,
        string FilePath
    );

    public record UpdateFileResourceDTO(
        string FileName,
        string FilePath
    );

    public record GetFileResourceDTO(
        Guid Id,
        string FileName,
        string FilePath,
        DateTime UploadedAt
    );


    public record CreateSystemSettingDTO(
        string Key,
        string Value
    );

    public record UpdateSystemSettingDTO(
        string Value
    );

    public record GetSystemSettingDTO(
        Guid Id,
        string Key,
        string Value
    );

}
