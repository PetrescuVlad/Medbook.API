namespace Medbook.Api.DTOs.Patients;

public class PatientDto
{
    public Guid PatientProfileId { get; set; }
    public Guid UserId { get; set; }

    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;

    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = null!;
}