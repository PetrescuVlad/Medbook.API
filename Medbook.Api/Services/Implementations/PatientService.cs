using Medbook.Api.Database;
using Medbook.Api.Database.Models;
using Medbook.Api.DTOs.Common;
using Medbook.Api.DTOs.Patients;
using Medbook.Api.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Medbook.Api.Services.Implementations;

public class PatientService : IPatientService
{
    private readonly MedbookDbContext _db;

    public PatientService(MedbookDbContext db)
    {
        _db = db;
    }

    public async Task<PatientDto?> GetByIdAsync(Guid patientProfileId, CancellationToken ct = default)
    {
        var pp = await _db.PatientProfiles
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Doctor)
            .FirstOrDefaultAsync(x => x.Id == patientProfileId, ct);

        return pp is null ? null : ToDto(pp);
    }

    public async Task<PagedResponseDto<PatientDto>> GetPagedAsync(
        PaginationQueryParams pagination,
        string? search = null,
        CancellationToken ct = default)
    {
        var query = _db.PatientProfiles
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Doctor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(p =>
                p.User.Email.ToLower().Contains(s) ||
                (p.User.FullName != null && p.User.FullName.ToLower().Contains(s)) ||
                p.Doctor.FullName.ToLower().Contains(s));
        }

        query = query.OrderBy(p => p.User.Email);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(p => ToDto(p))
            .ToListAsync(ct);

        return new PagedResponseDto<PatientDto>
        {
            Items = items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = total
        };
    }

    public async Task<PatientDto> CreateAsync(PatientAddDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new InvalidOperationException("Email is required.");

        if (string.IsNullOrWhiteSpace(dto.FullName))
            throw new InvalidOperationException("FullName is required.");

        var emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email, ct);
        if (emailExists)
            throw new InvalidOperationException("Email already exists.");

        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Id == dto.DoctorId, ct);
        if (doctor is null)
            throw new KeyNotFoundException("Doctor not found.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FullName = dto.FullName,
            PasswordHash = "demo",
            Role = UserRole.Patient
        };

        var profile = new PatientProfile
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DoctorId = doctor.Id
        };

        _db.Users.Add(user);
        _db.PatientProfiles.Add(profile);

        await _db.SaveChangesAsync(ct);

        var created = await _db.PatientProfiles
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Doctor)
            .FirstAsync(x => x.Id == profile.Id, ct);

        return ToDto(created);
    }

    private static PatientDto ToDto(PatientProfile pp) => new()
    {
        PatientProfileId = pp.Id,
        UserId = pp.UserId,
        Email = pp.User.Email,
        FullName = pp.User.FullName ?? "",
        DoctorId = pp.DoctorId,
        DoctorName = pp.Doctor.FullName
    };
}
