namespace ClinicMvc.ViewModels;

public class PatientDashboardViewModel
{
    public int UpcomingCount { get; set; }
    public int PendingCount { get; set; }
    public int CompletedCount { get; set; }
    public int TotalCount { get; set; }
    public string? MostVisitedDoctor { get; set; }
    public int MostVisitedCount { get; set; }
}