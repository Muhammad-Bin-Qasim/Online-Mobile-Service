using System.ComponentModel.DataAnnotations;

namespace OnlineMobileServices.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
