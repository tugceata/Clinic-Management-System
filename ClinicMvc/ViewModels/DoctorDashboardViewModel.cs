namespace ClinicMvc.ViewModels;

public class DoctorDashboardViewModel
{
    public int TodayCount { get; set; }
    public int PendingCount { get; set; }
    public int WeekPatientCount { get; set; }
    public int CompletedCount { get; set; }

    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int ThisMonthPatientCount { get; set; }
    public List<ReviewSummary> RecentReviews { get; set; } = new();

    public int MaxAppointments { get; set; }
    public int TodayUsed { get; set; }
    public int OccupancyPercent =>
        MaxAppointments == 0 ? 0 : (int)Math.Round((double)TodayUsed / MaxAppointments * 100);
}

public class ReviewSummary
{
    public string PatientName { get; set; } = "";
    public int Score { get; set; }
    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}