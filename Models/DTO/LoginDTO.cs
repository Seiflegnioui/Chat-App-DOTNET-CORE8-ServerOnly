using System.ComponentModel.DataAnnotations;
using p4.Models.Entities;

namespace p4.Models.DTO
{
    public class LoginDTO
    {
        public string email { get; set; }
        public string password { get; set; }

    }
}