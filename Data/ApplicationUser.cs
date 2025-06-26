using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WlodCar.Data.Entities;

namespace WlodCar.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(32)] public string? FirstName { get; set; }
        [MaxLength(32)] public string? LastName { get; set; }
        public string? AvatarFileName { get; set; }     // new
        public string? Note { get; set; }     // new
        public int LoyaltyPoints { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = [];
        public ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; } = [];
    }

}
