namespace Medbook.Api.Database.Models;

public enum UserRole
{
    Admin = 0,
    Doctor = 1,
    Patient = 2
}

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }

    // Navigation
    public Doctor? DoctorProfile { get; set; }

    public PatientProfile? PatientProfile { get; set; }
    
    public string? FullName { get; set; }

    public ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
}

