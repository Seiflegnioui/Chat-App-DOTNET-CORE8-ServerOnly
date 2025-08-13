using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using p4.Models.Entities;

namespace p4.Models.DTO
{
    public class MesssageDTO
    {

        
        public int ConversationId { get; set; }
        public string content { get; set; }


        
    }
}