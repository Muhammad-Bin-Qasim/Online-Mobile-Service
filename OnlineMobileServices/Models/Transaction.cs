using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMobileServices.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }   // 🔥 THIS IS REQUIRED

        [Required]
        [RegularExpression(@"^[0-9]{10}$")]
        public string MobileNumber { get; set; }

        public string TransactionType { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public string Status { get; set; }
    }
}