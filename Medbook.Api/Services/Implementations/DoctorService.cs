using Medbook.Api.Database;
using Medbook.Api.Database.Models;
using Medbook.Api.DTOs.Common;
using Medbook.Api.DTOs.Doctors;
using Medbook.Api.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Medbook.Api.Services.Implementations;

public class DoctorService : IDoctorService
{
    private readonly MedbookDbContext _db;

    public DoctorService(MedbookDbContext db)
    {
        _db = db;
    }

    public async Task<DoctorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var doctor = await _db.Doctors
            .AsNoTracking()
            .Include(d => d.DoctorSpecialties)
            .ThenInclude(ds => ds.Specialty)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        return doctor is null ? null : ToDto(doctor);
    }

    public async Task<PagedResponseDto<DoctorDto>> GetPagedAsync(
        PaginationQueryParams pagination,
        string? search = null,
        CancellationToken ct = default)
    {
        var query = _db.Doctors.AsNoTracking()
            .Include(d => d.DoctorSpecialties)
            .ThenInclude(ds => ds.Specialty)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(d => d.FullName.ToLower().Contains(s));
        }

        query = query.OrderBy(d => d.FullName);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(d => ToDto(d))
            .ToListAsync(ct);

        return new PagedResponseDto<DoctorDto>
        {
            Items = items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = total
        };
    }

    public async Task<DoctorDto> CreateAsync(DoctorAddDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
            throw new InvalidOperationException("FullName is required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new InvalidOperationException("Email is required.");

        var emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email, ct);
        if (emailExists)
            throw new InvalidOperationException("Email already exists.");

        var specialtyIds = dto.SpecialtyIds.Distinct().ToList();
        if (specialtyIds.Count > 0)
        {
            var existingCount = await _db.Specialties.CountAsync(s => specialtyIds.Contains(s.Id), ct);
            if (existingCount != specialtyIds.Count)
                throw new KeyNotFoundException("One or more specialties not found.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = "demo",
            Role = UserRole.Doctor
        };

        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FullName = dto.FullName
        };

        _db.Users.Add(user);
        _db.Doctors.Add(doctor);

        foreach (var sid in specialtyIds)
        {
            _db.DoctorSpecialties.Add(new DoctorSpecialty
            {
                DoctorId = doctor.Id,
                SpecialtyId = sid
            });
        }

        await _db.SaveChangesAsync(ct);

        var created = await _db.Doctors
            .AsNoTracking()
            .Include(d => d.DoctorSpecialties)
            .ThenInclude(ds => ds.Specialty)
            .FirstAsync(d => d.Id == doctor.Id, ct);

        return ToDto(created);
    }

    private static DoctorDto ToDto(Doctor d) => new()
    {
        Id = d.Id,
        UserId = d.UserId,
        FullName = d.FullName,
        Specialties = d.DoctorSpecialties.Select(ds => ds.Specialty.Name).ToList()
    };
}
