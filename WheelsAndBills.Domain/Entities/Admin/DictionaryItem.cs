namespace WheelsAndBills.Domain.Entities.Admin
{
    public class DictionaryItem
    {
        public Guid Id { get; set; }
        public Guid DictionaryId { get; set; }
        public string Value { get; set; } = null!;
    }

}
