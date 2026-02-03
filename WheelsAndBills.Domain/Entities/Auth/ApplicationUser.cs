using Microsoft.AspNetCore.Identity;
using WheelsAndBills.Domain.Entities.Report;
using WheelsAndBills.Domain.Entities.Vehicles;

namespace WheelsAndBills.Domain.Entities.Auth
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<WheelsAndBills.Domain.Entities.Report.Report> Reports { get; set; } = new List<WheelsAndBills.Domain.Entities.Report.Report>();
    }
}


