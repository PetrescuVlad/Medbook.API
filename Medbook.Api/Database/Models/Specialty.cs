namespace Medbook.Api.Database.Models;

public class Specialty
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<DoctorSpecialty> DoctorSpecialties { get; set; } = new List<DoctorSpecialty>();
}