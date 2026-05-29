using System.ComponentModel.DataAnnotations;

namespace ClinicMvc.Models;

public class Department
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Branş adı zorunludur.")]
    [StringLength(60)]
    public string Name { get; set; } = string.Empty;

    // Bir branşta birden çok doktor olur (1 - N)
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
