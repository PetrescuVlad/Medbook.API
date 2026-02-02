namespace Medbook.Api.DTOs.Patients;

public class PatientAddDto
{
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public Guid DoctorId { get; set; }
}