using System.ComponentModel.DataAnnotations;
using p4.Models.Entities;

namespace p4.Models.DTO
{
    public class UsersDTO
    {
        
        public int Id { get; set; }
        public string email { get; set; }

        
        public Role role { get; set; }
    }
}