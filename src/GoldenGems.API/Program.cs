using GoldenGems.API.Hubs;
using GoldenGems.API.Middleware;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Infrastructure;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

// Add services to the container
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddSignalR();

// Register all Infrastructure + Application services
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS (AllowCredentials required for SignalR)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "GoldenGems API",
        Version = "v1",
        Description = "API Backend para GoldenGems"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoldenGems API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Ensure wwwroot exists for static files (profile photos, product images)
var wwwrootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(wwwrootPath);
app.Environment.WebRootPath = wwwrootPath;

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

// Initialize default roles and admin user
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GoldenGemsDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

    // Create "User" role if it doesn't exist
    if (!context.Roles.Any(r => r.Name.ToLower() == "user"))
    {
        context.Roles.Add(new Role
        {
            Id = Guid.NewGuid(),
            Name = "User",
            Description = "Rol por defecto para usuarios registrados",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

    // Create "Admin" role if it doesn't exist
    if (!context.Roles.Any(r => r.Name.ToLower() == "admin"))
    {
        context.Roles.Add(new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Description = "Rol de administrador con acceso total",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

    // Seed document types (Colombian)
    var documentTypes = new[]
    {
        new { Code = "CC", Name = "Cédula de Ciudadanía" },
        new { Code = "CE", Name = "Cédula de Extranjería" },
        new { Code = "TI", Name = "Tarjeta de Identidad" },
        new { Code = "PA", Name = "Pasaporte" },
        new { Code = "NIT", Name = "Número de Identificación Tributaria" },
        new { Code = "PPT", Name = "Permiso por Protección Temporal" },
    };

    foreach (var dt in documentTypes)
    {
        if (!context.DocumentTypes.Any(d => d.Code.ToLower() == dt.Code.ToLower()))
        {
            context.DocumentTypes.Add(new DocumentType
            {
                Id = Guid.NewGuid(),
                Code = dt.Code,
                Name = dt.Name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
    await context.SaveChangesAsync();

    // Create admin user if it doesn't exist
    if (!await context.Users.AnyAsync(u => u.Username.ToLower() == "admin"))
    {
        var adminRole = await context.Roles.FirstAsync(r => r.Name.ToLower() == "admin");
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@goldengems.com",
            Username = "admin",
            PasswordHash = passwordHasher.HashPassword("Admin123*"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();

        context.Set<UserRole>().Add(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = adminUser.Id,
            RoleId = adminRole.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }
}

app.Run();
