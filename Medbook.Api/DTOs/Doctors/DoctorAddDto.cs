namespace Medbook.Api.DTOs.Doctors;

public class DoctorAddDto
{
    public string FullName { get; set; } = null!;
    public List<Guid> SpecialtyIds { get; set; } = new();
    public string Email { get; set; } = null!;
}