using Medbook.Api.Database;
using Medbook.Api.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Medbook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecialtiesController : ControllerBase
{
    private readonly MedbookDbContext _db;

    public SpecialtiesController(MedbookDbContext db)
    {
        _db = db;
    }

    // GET: api/specialties
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var specialties = await _db.Specialties
            .Select(s => new { s.Id, s.Name })
            .ToListAsync();

        return Ok(specialties);
    }

    // GET: api/specialties/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var specialty = await _db.Specialties
            .Where(s => s.Id == id)
            .Select(s => new { s.Id, s.Name })
            .FirstOrDefaultAsync();

        if (specialty == null)
            return NotFound("Specialty not found.");

        return Ok(specialty);
    }

    // POST: api/specialties
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSpecialtyRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");

        var name = request.Name.Trim();

        var exists = await _db.Specialties.AnyAsync(s => s.Name == name);
        if (exists)
            return Conflict("Specialty already exists.");

        var specialty = new Specialty
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        _db.Specialties.Add(specialty);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = specialty.Id }, new
        {
            specialty.Id,
            specialty.Name
        });
    }

    // DTO local doar pentru request (nu mai depinzi de CreateSpecialtyDto)
    public class CreateSpecialtyRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
