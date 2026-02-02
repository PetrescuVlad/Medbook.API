using Medbook.Api.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Medbook.Api.Database;

public class MedbookDbContext : DbContext
{
    public MedbookDbContext(DbContextOptions<MedbookDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<DoctorSpecialty> DoctorSpecialties => Set<DoctorSpecialty>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<PatientProfile> PatientProfiles => Set<PatientProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // User (DoctorProfile) 1-1 Doctor
        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.User)
            .WithOne(u => u.DoctorProfile)
            .HasForeignKey<Doctor>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // DoctorSpecialty composite key (N-N)
        modelBuilder.Entity<DoctorSpecialty>()
            .HasKey(ds => new { ds.DoctorId, ds.SpecialtyId });

        modelBuilder.Entity<DoctorSpecialty>()
            .HasOne(ds => ds.Doctor)
            .WithMany(d => d.DoctorSpecialties)
            .HasForeignKey(ds => ds.DoctorId);

        modelBuilder.Entity<DoctorSpecialty>()
            .HasOne(ds => ds.Specialty)
            .WithMany(s => s.DoctorSpecialties)
            .HasForeignKey(ds => ds.SpecialtyId);

        // Appointment relationships
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(u => u.PatientAppointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Helpful index for conflict checks
        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.DoctorId, a.StartTime, a.EndTime });
        // User (PatientProfile) 1-1 PatientProfile
        modelBuilder.Entity<PatientProfile>()
            .HasOne(pp => pp.User)
            .WithOne(u => u.PatientProfile)
            .HasForeignKey<PatientProfile>(pp => pp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

// PatientProfile -> Doctor (many-to-one)
        modelBuilder.Entity<PatientProfile>()
            .HasOne(pp => pp.Doctor)
            .WithMany()
            .HasForeignKey(pp => pp.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
