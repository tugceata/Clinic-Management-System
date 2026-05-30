using ClinicMvc.Models;

namespace ClinicMvc.ViewModels;

public class AppointmentDetailViewModel
{
    public Appointment Appointment { get; set; } = null!;
    public List<Appointment> History { get; set; } = new();
}