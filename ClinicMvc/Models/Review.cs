using System.ComponentModel.DataAnnotations;

namespace ClinicMvc.Models;

// Doctor puanı bu kayıtlardan LINQ ile ortalanır (Doctor.Rating diye bir alan tutmuyoruz)
public class Review
{
    public int Id { get; set; }

    [Required]
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    [Required]
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    [Range(1, 5, ErrorMessage = "Puan 1-5 arası olmalı.")]
    [Display(Name = "Puan")]
    public int Score { get; set; }

    [StringLength(300)]
    [Display(Name = "Yorum")]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
