namespace Medbook.Api.DTOs.Doctors;

public class DoctorDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public List<string> Specialties { get; set; } = new();
}