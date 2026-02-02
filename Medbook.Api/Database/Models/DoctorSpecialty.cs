namespace Medbook.Api.Database.Models;

public class DoctorSpecialty
{
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public Guid SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = null!;
}