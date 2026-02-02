using Medbook.Api.DTOs.Common;
using Medbook.Api.DTOs.Doctors;
using Medbook.Api.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Medbook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _service;

    public DoctorsController(IDoctorService service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
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
    public async Task<IActionResult> Create([FromBody] DoctorAddDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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