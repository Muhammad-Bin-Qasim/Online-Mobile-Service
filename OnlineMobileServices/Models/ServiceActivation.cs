namespace OnlineMobileServices.Models;
using System.ComponentModel.DataAnnotations;

public class ServiceActivation
{
    [Key]
    public int ServiceId { get; set; }

    public string UserId { get; set; }

    public string ServiceName { get; set; }

    public DateTime ActivationDate { get; set; } = DateTime.Now;

    public string Status { get; set; }
}
