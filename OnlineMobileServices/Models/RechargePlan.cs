namespace OnlineMobileServices.Models;
using System.ComponentModel.DataAnnotations;

public class RechargePlan
{
    [Key]   // ⭐ IMPORTANT
    public int PlanId { get; set; }

    [Required]
    public string PlanType { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; }
}
