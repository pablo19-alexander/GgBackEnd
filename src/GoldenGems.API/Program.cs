using GoldenGems.API.Middleware;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Infrastructure;
using GoldenGems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

// Add services to the container
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

// Register all Infrastructure + Application services
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

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
