namespace WheelsAndBillsAPI.Domain.Entities.Admin
{
    public class ContentPage
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = null!;
        public string Title { get; set; } = null!;
    }
}
