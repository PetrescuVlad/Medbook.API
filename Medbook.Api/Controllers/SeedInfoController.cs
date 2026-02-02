using Medbook.Api.Database;
using Medbook.Api.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Medbook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedInfoController : ControllerBase
{
    private readonly MedbookDbContext _db;

    public SeedInfoController(MedbookDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var patient = await _db.Users.AsNoTracking()
            .FirstAsync(u => u.Role == UserRole.Patient, ct);

        var doctor = await _db.Doctors.AsNoTracking()
            .FirstAsync(ct);

        return Ok(new
        {
            patientId = patient.Id,
            doctorId = doctor.Id
        });
    }
}