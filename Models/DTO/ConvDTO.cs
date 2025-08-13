using System.ComponentModel.DataAnnotations;
using p4.Models.Entities;

namespace p4.Models.DTO
{
    public class ConvDTO
    {
        
        public int ReceiverId { get; set; }
        public DateTime last_join { get; set; }

        
    }
}