namespace ClinicMvc.Models;

public static class StatusHelper
{
    public static string ToTr(this AppointmentStatus s) => s switch
    {
        AppointmentStatus.Pending   => "Onay Bekliyor",
        AppointmentStatus.Approved  => "Onaylandı",
        AppointmentStatus.Completed => "Tamamlandı",
        AppointmentStatus.Cancelled => "İptal",
        _ => s.ToString()
    };
}