using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace p4.Models.Entities
{
    public class Msg
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [NotNull]
        [ForeignKey("ConversationID")]
        public int ConversationID { get; set; }
        public Conversation Conversation { get; set; }

        [Required]
        [NotNull]
        [ForeignKey("SenderId")]
        public int SenderId { get; set; }
        public User Sender { get; set; }

        [NotNull]
        [ForeignKey("ReceiverId")]
        [Required]
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }

        [Required]
        public string content { get; set; }

        [Required]
        public DateTime time { get; set; } = DateTime.Now;
        
        public DateTime? seen_time { get; set; } = null;
    }
}