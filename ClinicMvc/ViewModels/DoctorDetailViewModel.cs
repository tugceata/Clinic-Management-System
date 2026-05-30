using ClinicMvc.Models;

namespace ClinicMvc.ViewModels;

public class DoctorDetailViewModel
{
    public Doctor Doctor { get; set; } = null!;
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<Review> Reviews { get; set; } = new();
    public bool HasCompleted { get; set; }
    public bool HasReview { get; set; }
}