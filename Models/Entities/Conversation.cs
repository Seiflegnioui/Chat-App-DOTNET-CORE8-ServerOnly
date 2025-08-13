using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace p4.Models.Entities
{
    public class Conversation
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [NotNull]
        [ForeignKey("SenderId")]
        public int SenderId { get; set; }
        public User Sender { get; set; }

        [NotNull]
        [ForeignKey("SenderId")]
        [Required]
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }

        [Required]
        public DateTime last_join { get; set; } = DateTime.Now;
    }
}