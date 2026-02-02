using Medbook.Api.DTOs.Appointments;
using Medbook.Api.DTOs.Common;

namespace Medbook.Api.Services.Abstractions;

public interface IAppointmentService
{
    Task<AppointmentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<PagedResponseDto<AppointmentDto>> GetPagedAsync(
        PaginationQueryParams pagination,
        Guid? doctorId = null,
        Guid? patientId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Creează o programare. Trebuie să valideze:
    /// - Start < End
    /// - Doctor și Patient existenți
    /// - fără suprapuneri pentru același doctor (doar Active)
    /// </summary>
    Task<AppointmentDto> CreateAsync(AppointmentAddDto dto, CancellationToken ct = default);

    /// <summary>
    /// Update simplu: mutare interval și/sau anulare.
    /// Pentru mutare trebuie revalidată suprapunerea.
    /// </summary>
    Task<AppointmentDto?> UpdateAsync(Guid id, AppointmentUpdateDto dto, CancellationToken ct = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}