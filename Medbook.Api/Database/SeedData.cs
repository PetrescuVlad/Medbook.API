using Medbook.Api.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Medbook.Api.Database;

public static class SeedData
{
    public static async Task EnsureSeededAsync(MedbookDbContext db, CancellationToken ct = default)
    {
        // dacă există deja useri, considerăm seeded
        if (await db.Users.AnyAsync(ct))
            return;

        var patient = new User
        {
            Id = Guid.NewGuid(),
            Email = "patient@medbook.local",
            PasswordHash = "demo", // temporar (când facem Auth, va fi hash real)
            Role = UserRole.Patient
        };

        var doctorUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "doctor@medbook.local",
            PasswordHash = "demo",
            Role = UserRole.Doctor
        };

        var specialty = new Specialty
        {
            Id = Guid.NewGuid(),
            Name = "Cardiology"
        };

        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            UserId = doctorUser.Id,
            FullName = "Dr. Demo Doctor"
        };

        var doctorSpecialty = new DoctorSpecialty
        {
            DoctorId = doctor.Id,
            SpecialtyId = specialty.Id
        };

        db.Users.AddRange(patient, doctorUser);
        db.Specialties.Add(specialty);
        db.Doctors.Add(doctor);
        db.DoctorSpecialties.Add(doctorSpecialty);

        await db.SaveChangesAsync(ct);
    }
}