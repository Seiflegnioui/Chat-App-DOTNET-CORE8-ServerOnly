using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace p4.Models.Entities
{
    [Index("username",IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The email is required")]
        [MaxLength(50, ErrorMessage = "Must be less than or equal to 50 characters")]
        public string Email { get; set; }

        public string username { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public Role role { get; set; } = Role.User;

        [Required(ErrorMessage = "Password is required")]
        public string HashedPassword { get; set; }

        public string photo { get; set; }
        public DateTime last_seen { get; set; }
    }

    public enum Role
    {
        Admin,
        User,
        Guest
    }
}
