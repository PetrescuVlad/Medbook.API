namespace Medbook.Api.Database.Models;

public class Doctor
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public ICollection<DoctorSpecialty> DoctorSpecialties { get; set; } = new List<DoctorSpecialty>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}