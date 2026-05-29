using System.ComponentModel.DataAnnotations;

namespace ClinicMvc.Models;

public class Prescription
{
    public int Id { get; set; }

    [Required]
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    [Required(ErrorMessage = "İlaç adı zorunludur.")]
    [StringLength(100)]
    [Display(Name = "İlaç")]
    public string Medication { get; set; } = string.Empty;

    [StringLength(60)]
    [Display(Name = "Doz")]
    public string Dosage { get; set; } = string.Empty;

    [StringLength(300)]
    [Display(Name = "Kullanım Talimatı")]
    public string Instructions { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Tarih")]
    public DateTime IssuedDate { get; set; } = DateTime.Now;
}
