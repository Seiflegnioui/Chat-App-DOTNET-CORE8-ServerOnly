using System.ComponentModel.DataAnnotations;

namespace p4.Models.DTO
{
    public class UserDTO
    {
        [Required(ErrorMessage = "the email is required")]
        [MaxLength(length:50,ErrorMessage = "must be less than 50")]
        public string email { get; set; }

        [Required(ErrorMessage = "the USERNAME is required")]

        public string username { get; set; }

        public IFormFile photo { get; set; }
        public DateTime last_seen { get; set; }
        [Required(ErrorMessage = "the password is required")]
        [MaxLength(length:50,ErrorMessage = "must be less than 50")]
        public string password { get; set; }
    }
}