using Medbook.Api.DTOs.Common;
using Medbook.Api.DTOs.Patients;

namespace Medbook.Api.Services.Abstractions;

public interface IPatientService
{
    Task<PatientDto?> GetByIdAsync(Guid patientProfileId, CancellationToken ct = default);

    Task<PagedResponseDto<PatientDto>> GetPagedAsync(
        PaginationQueryParams pagination,
        string? search = null,
        CancellationToken ct = default);

    Task<PatientDto> CreateAsync(PatientAddDto dto, CancellationToken ct = default);
}