namespace ClinicMvc.ViewModels;

public class HomeIndexViewModel
{
    public string? Role { get; set; }
    public string? Name { get; set; }
    public DoctorDashboardViewModel? Doctor { get; set; }
    public PatientDashboardViewModel? Patient { get; set; }
}