using GoldenGems.Domain.Entities;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Entities.People;
using Microsoft.EntityFrameworkCore;

namespace GoldenGems.Infrastructure.Data;

public class GoldenGemsDbContext : DbContext
{
    public GoldenGemsDbContext(DbContextOptions<GoldenGemsDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;
    public DbSet<Module> Modules { get; set; } = default!;
    public DbSet<Form> Forms { get; set; } = default!;
    public DbSet<Actions> Actions { get; set; } = default!;
    public DbSet<ActionType> ActionTypes { get; set; } = default!;
    public DbSet<RoleAction> RoleActions { get; set; } = default!;
    public DbSet<UserRole> UserRoles { get; set; } = default!;
    public DbSet<Person> People { get; set; } = default!;
    public DbSet<Contact> Contacts { get; set; } = default!;
    public DbSet<Region> Regions { get; set; } = default!;
    public DbSet<DocumentType> DocumentTypes { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique indexes
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
        modelBuilder.Entity<Module>().HasIndex(m => m.Code).IsUnique();
        modelBuilder.Entity<Form>().HasIndex(f => f.Code).IsUnique();
        modelBuilder.Entity<Actions>().HasIndex(a => a.Code).IsUnique();
        modelBuilder.Entity<ActionType>().HasIndex(at => at.Code).IsUnique();

        // Form -> Module
        modelBuilder.Entity<Form>()
            .HasOne(f => f.Module)
            .WithMany(m => m.Forms)
            .HasForeignKey(f => f.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Actions -> Module
        modelBuilder.Entity<Actions>()
            .HasOne(a => a.Module)
            .WithMany(m => m.Actions)
            .HasForeignKey(a => a.ModuleId)
            .OnDelete(DeleteBehavior.SetNull);

        // Actions -> Form
        modelBuilder.Entity<Actions>()
            .HasOne(a => a.Form)
            .WithMany(f => f.Actions)
            .HasForeignKey(a => a.FormId)
            .OnDelete(DeleteBehavior.SetNull);

        // Actions -> ActionType
        modelBuilder.Entity<Actions>()
            .HasOne(a => a.ActionType)
            .WithMany(at => at.Actions)
            .HasForeignKey(a => a.ActionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserRole -> User
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserRole -> Role
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // RoleAction -> Role
        modelBuilder.Entity<RoleAction>()
            .HasOne(ra => ra.Role)
            .WithMany(r => r.RoleActions)
            .HasForeignKey(ra => ra.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // RoleAction -> Action
        modelBuilder.Entity<RoleAction>()
            .HasOne(ra => ra.Action)
            .WithMany(a => a.RoleActions)
            .HasForeignKey(ra => ra.ActionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Person -> User (1:1)
        modelBuilder.Entity<Person>()
            .HasOne(p => p.User)
            .WithOne(u => u.Person)
            .HasForeignKey<Person>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Person -> DocumentType
        modelBuilder.Entity<Person>()
            .HasOne(p => p.DocumentType)
            .WithMany(dt => dt.People)
            .HasForeignKey(p => p.DocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Person -> Contact
        modelBuilder.Entity<Person>()
            .HasOne(p => p.Contact)
            .WithMany(c => c.People)
            .HasForeignKey(p => p.ContactId)
            .OnDelete(DeleteBehavior.SetNull);

        // Contact -> Region
        modelBuilder.Entity<Contact>()
            .HasOne(c => c.Region)
            .WithMany(r => r.Contacts)
            .HasForeignKey(c => c.RegionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
