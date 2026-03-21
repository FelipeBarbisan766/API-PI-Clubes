using API_PI_Clubes.Model.Enums;
using Microsoft.AspNetCore.Identity;

namespace API_PI_Clubes.Model
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public RoleEnum Role { get; set; }

        public ICollection<Admin> Admins { get; set; }
        public ICollection<Player> Players { get; set; }
    }
}
