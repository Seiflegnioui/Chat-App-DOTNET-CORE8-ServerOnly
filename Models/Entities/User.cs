using System.ComponentModel.DataAnnotations;

namespace p4.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The email is required")]
        [MaxLength(50, ErrorMessage = "Must be less than or equal to 50 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public Role role { get; set; } = Role.User;

        [Required(ErrorMessage = "Password is required")]
        public string HashedPassword { get; set; }
    }

    public enum Role
    {
        Admin,
        User,
        Guest
    }
}
