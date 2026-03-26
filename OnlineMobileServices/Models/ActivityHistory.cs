namespace OnlineMobileServices.Models;
using System.ComponentModel.DataAnnotations;

public class ActivityHistory
{
    [Key]
    public int Id { get; set; }

    public string? UserId { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public string ActivityType { get; set; }

    public string Description { get; set; }

    public decimal? Amount { get; set; }

    public string Status { get; set; }
}

