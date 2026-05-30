using System.ComponentModel.DataAnnotations;

namespace ClinicMvc.Models;

// Doctor N — N Patient ilişkisinin join tablosu
public class Appointment
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Doktor")]
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    [Required]
    [Display(Name = "Hasta")]
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    [Required(ErrorMessage = "Randevu tarihi zorunludur.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Randevu Tarihi")]
    public DateTime AppointmentDate { get; set; }

    [Display(Name = "Durum")]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    [StringLength(300)]
    [Display(Name = "Not")]
    public string Notes { get; set; } = string.Empty;

    [StringLength(300)]
    [Display(Name = "Teşhis")]
    public string Diagnosis { get; set; } = string.Empty;

    // Bir randevuya reçete(ler) bağlanabilir
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
