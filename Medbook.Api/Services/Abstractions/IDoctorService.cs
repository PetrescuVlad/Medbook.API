using Medbook.Api.DTOs.Common;
using Medbook.Api.DTOs.Doctors;

namespace Medbook.Api.Services.Abstractions;

public interface IDoctorService
{
    Task<DoctorDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<PagedResponseDto<DoctorDto>> GetPagedAsync(
        PaginationQueryParams pagination,
        string? search = null,
        CancellationToken ct = default);

    Task<DoctorDto> CreateAsync(DoctorAddDto dto, CancellationToken ct = default);
}