using Medbook.Api.Database;
using Medbook.Api.Database.Models;
using Medbook.Api.DTOs.Appointments;
using Medbook.Api.DTOs.Common;
using Medbook.Api.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Medbook.Api.Services.Implementations;

public class AppointmentService : IAppointmentService
{
    private readonly MedbookDbContext _db;

    public AppointmentService(MedbookDbContext db)
    {
        _db = db;
    }

    public async Task<AppointmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<PagedResponseDto<AppointmentDto>> GetPagedAsync(
        PaginationQueryParams pagination,
        Guid? doctorId = null,
        Guid? patientId = null,
        CancellationToken ct = default)
    {
        var query = _db.Appointments.AsNoTracking().AsQueryable();

        if (doctorId.HasValue)
            query = query.Where(a => a.DoctorId == doctorId.Value);

        if (patientId.HasValue)
            query = query.Where(a => a.PatientId == patientId.Value);

        // sort: newest first
        query = query.OrderByDescending(a => a.CreatedAt);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(a => ToDto(a))
            .ToListAsync(ct);

        return new PagedResponseDto<AppointmentDto>
        {
            Items = items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = total
        };
    }

    public async Task<AppointmentDto> CreateAsync(AppointmentAddDto dto, CancellationToken ct = default)
    {
        if (dto.StartTime >= dto.EndTime)
            throw new InvalidOperationException("StartTime must be earlier than EndTime.");

        // Doctor exists
        var doctorExists = await _db.Doctors.AnyAsync(d => d.Id == dto.DoctorId, ct);
        if (!doctorExists)
            throw new KeyNotFoundException("Doctor not found.");

        // Patient exists
        var patientExists = await _db.Users.AnyAsync(u => u.Id == dto.PatientId, ct);
        if (!patientExists)
            throw new KeyNotFoundException("Patient not found.");

        // Conflict check (only Active)
        var hasConflict = await HasDoctorConflictAsync(dto.DoctorId, dto.StartTime, dto.EndTime, excludeAppointmentId: null, ct);
        if (hasConflict)
            throw new InvalidOperationException("Appointment overlaps an existing one for this doctor.");

        var entity = new Appointment
        {
            Id = Guid.NewGuid(),
            DoctorId = dto.DoctorId,
            PatientId = dto.PatientId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = AppointmentStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _db.Appointments.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ToDto(entity);
    }

    public async Task<AppointmentDto?> UpdateAsync(Guid id, AppointmentUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (entity is null)
            return null;

        if (dto.Cancel == true)
        {
            entity.Status = AppointmentStatus.Cancelled;
            await _db.SaveChangesAsync(ct);
            return ToDto(entity);
        }

        var newStart = dto.StartTime ?? entity.StartTime;
        var newEnd = dto.EndTime ?? entity.EndTime;

        if (newStart >= newEnd)
            throw new InvalidOperationException("StartTime must be earlier than EndTime.");

        // Only re-check conflicts if appointment stays active
        if (entity.Status == AppointmentStatus.Active)
        {
            var hasConflict = await HasDoctorConflictAsync(entity.DoctorId, newStart, newEnd, excludeAppointmentId: entity.Id, ct);
            if (hasConflict)
                throw new InvalidOperationException("Appointment overlaps an existing one for this doctor.");
        }

        entity.StartTime = newStart;
        entity.EndTime = newEnd;

        await _db.SaveChangesAsync(ct);

        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (entity is null)
            return false;

        _db.Appointments.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<bool> HasDoctorConflictAsync(
        Guid doctorId,
        DateTime start,
        DateTime end,
        Guid? excludeAppointmentId,
        CancellationToken ct)
    {
        // Overlap rule (allow touching ends): start < existingEnd AND existingStart < end
        var query = _db.Appointments.AsNoTracking()
            .Where(a => a.DoctorId == doctorId)
            .Where(a => a.Status == AppointmentStatus.Active)
            .Where(a => start < a.EndTime && a.StartTime < end);

        if (excludeAppointmentId.HasValue)
            query = query.Where(a => a.Id != excludeAppointmentId.Value);

        return await query.AnyAsync(ct);
    }

    private static AppointmentDto ToDto(Appointment a) => new()
    {
        Id = a.Id,
        DoctorId = a.DoctorId,
        PatientId = a.PatientId,
        StartTime = a.StartTime,
        EndTime = a.EndTime,
        Status = a.Status,
        CreatedAt = a.CreatedAt
    };
}
