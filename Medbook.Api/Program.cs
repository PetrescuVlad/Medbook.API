using Microsoft.OpenApi.Models;
using Medbook.Api.Database;
using Microsoft.EntityFrameworkCore;
using Medbook.Api.Services.Abstractions;
using Medbook.Api.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddDbContext<MedbookDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<Medbook.Api.Services.Abstractions.IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MedBook API",
        Version = "v1"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MedbookDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.EnsureSeededAsync(db);
}

app.Run();