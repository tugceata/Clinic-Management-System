using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicMvc.Models;

public class Patient
{
    
    public int Id { get; set; }

    [Required(ErrorMessage = "İsim zorunludur.")]
    [StringLength(80)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Geçerli bir telefon giriniz.")]
    [Display(Name = "Telefon")]
    public string Phone { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Doğum Tarihi")]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "Cinsiyet")]
    public Gender Gender { get; set; }

    [NotMapped]
public int Age
{
    get
    {
        var t = DateTime.Today;
        var age = t.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > t.AddYears(-age)) age--;
        return age;
    }
}

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
