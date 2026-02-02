using Medbook.Api.DTOs.Common;
using Medbook.Api.DTOs.Patients;
using Medbook.Api.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Medbook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientsController(IPatientService service)
    {
        _service = service;
    }

    [HttpGet("{patientProfileId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid patientProfileId, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(patientProfileId, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] PaginationQueryParams pagination,
        [FromQuery] string? search,
        CancellationToken ct)
    {
        var result = await _service.GetPagedAsync(pagination, search, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PatientAddDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { patientProfileId = created.PatientProfileId }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}