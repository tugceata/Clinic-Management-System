using System.ComponentModel.DataAnnotations;

namespace ClinicMvc.Models;

public class Notification
{
    public int Id { get; set; }

    [Required] public string TargetRole { get; set; } = "";  // "Doctor" | "Patient"
    [Required] public int TargetUserId { get; set; }

    [Required, StringLength(300)]
    public string Message { get; set; } = "";

    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}