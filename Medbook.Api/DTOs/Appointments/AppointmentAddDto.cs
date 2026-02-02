namespace Medbook.Api.DTOs.Appointments;

public class AppointmentAddDto
{
    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}