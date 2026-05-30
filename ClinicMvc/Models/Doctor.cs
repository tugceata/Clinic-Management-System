using System.ComponentModel.DataAnnotations;

namespace ClinicMvc.Models;

public class Doctor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "İsim zorunludur.")]
    [StringLength(80)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Doktor no zorunludur.")]
    [StringLength(20)]
    [Display(Name = "Doktor No")]
    public string DoctorNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Branş seçiniz.")]
    [Display(Name = "Branş")]
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    [Display(Name = "Oda No")]
    [StringLength(10)]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(1, 100, ErrorMessage = "Randevu limiti 1-100 arası olmalı.")]
    [Display(Name = "Maksimum Randevu")]
    public int MaxAppointments { get; set; } = 10;

    [Range(0, 23)]
    [Display(Name = "Mesai Başlangıç")]
    public int WorkStartHour { get; set; } = 9;

    [Range(0, 23)]
    [Display(Name = "Mesai Bitiş")]
    public int WorkEndHour { get; set; } = 17;

    [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Hakkında")]
    public string Bio { get; set; } = string.Empty;

    [Display(Name = "Aktif mi?")]
    public bool IsActive { get; set; } = true;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
