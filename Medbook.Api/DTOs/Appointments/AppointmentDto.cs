using Medbook.Api.Database.Models;

namespace Medbook.Api.DTOs.Appointments;

public class AppointmentDto
{
    public Guid Id { get; set; }

    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public AppointmentStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}