namespace Medbook.Api.DTOs.Appointments;

public class AppointmentUpdateDto
{
    // Optional fields - dacă nu sunt trimise, nu modificăm
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    // Allow cancel through update (simplu)
    public bool? Cancel { get; set; }
}