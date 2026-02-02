namespace Medbook.Api.Database.Models;

public enum AppointmentStatus
{
    Active = 0,
    Cancelled = 1
}

public class Appointment
{
    public Guid Id { get; set; }

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public Guid PatientId { get; set; }
    public User Patient { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}