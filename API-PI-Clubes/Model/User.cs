using API_PI_Clubes.Model.Enums;
using API_PI_Clubes.Model.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace API_PI_Clubes.Model
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public EmailVerificationVO EmailVerification { get; set; }
        public ResetPasswordVO ResetPassword { get; set; }
        public RoleEnum Role { get; set; }

        public virtual ICollection<Admin> Admins { get; set; }
        public virtual ICollection<Player> Players { get; set; }
    }
}
