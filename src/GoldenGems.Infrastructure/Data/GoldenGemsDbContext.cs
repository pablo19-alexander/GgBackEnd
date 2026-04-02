using GoldenGems.Domain.Entities;
using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Entities.Chat;
using GoldenGems.Domain.Entities.Payment;
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
    public DbSet<Company> Companies { get; set; } = default!;
    public DbSet<ProductType> ProductTypes { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<ProductImage> ProductImages { get; set; } = default!;
    public DbSet<UserPreference> UserPreferences { get; set; } = default!;
    public DbSet<Conversation> Conversations { get; set; } = default!;
    public DbSet<Message> Messages { get; set; } = default!;
    public DbSet<Commission> Commissions { get; set; } = default!;
    public DbSet<Order> Orders { get; set; } = default!;
    public DbSet<GoldenGems.Domain.Entities.Payment.Payment> Payments { get; set; } = default!;

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

        // Company
        modelBuilder.Entity<Company>().HasIndex(c => c.Name).IsUnique();
        modelBuilder.Entity<Company>().HasIndex(c => c.NIT).IsUnique();
        modelBuilder.Entity<Company>()
            .HasOne(c => c.Owner)
            .WithMany()
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ProductType
        modelBuilder.Entity<ProductType>().HasIndex(pt => pt.Code).IsUnique();

        // Product
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Company)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductType)
            .WithMany(pt => pt.Products)
            .HasForeignKey(p => p.ProductTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .Property(p => p.ReferencePrice)
            .HasPrecision(18, 2);

        // ProductImage
        modelBuilder.Entity<ProductImage>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserPreference
        modelBuilder.Entity<UserPreference>().HasIndex(up => up.UserId).IsUnique();
        modelBuilder.Entity<UserPreference>()
            .HasOne(up => up.User)
            .WithMany()
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserPreference>()
            .HasOne(up => up.PreferredCompany)
            .WithMany()
            .HasForeignKey(up => up.PreferredCompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        // Conversation
        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Buyer)
            .WithMany()
            .HasForeignKey(c => c.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Seller)
            .WithMany()
            .HasForeignKey(c => c.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Product)
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Company)
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Conversation>()
            .Property(c => c.AgreedPrice)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Conversation>()
            .Property(c => c.Status)
            .HasConversion<string>();

        // Message
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Message>()
            .Property(m => m.OfferedPrice)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Message>()
            .Property(m => m.MessageType)
            .HasConversion<string>();

        // Commission
        modelBuilder.Entity<Commission>().HasIndex(c => c.CompanyId).IsUnique();
        modelBuilder.Entity<Commission>()
            .HasOne(c => c.Company)
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Commission>()
            .Property(c => c.Percentage)
            .HasPrecision(5, 2);
        modelBuilder.Entity<Commission>()
            .Property(c => c.MinAmount)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Commission>()
            .Property(c => c.MaxAmount)
            .HasPrecision(18, 2);
        // Order
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Conversation)
            .WithMany()
            .HasForeignKey(o => o.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Buyer)
            .WithMany()
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Seller)
            .WithMany()
            .HasForeignKey(o => o.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Product)
            .WithMany()
            .HasForeignKey(o => o.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Company)
            .WithMany()
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>().Property(o => o.AgreedPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.CommissionPercentage).HasPrecision(5, 2);
        modelBuilder.Entity<Order>().Property(o => o.CommissionAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.SellerAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.Status).HasConversion<string>();
        modelBuilder.Entity<Order>().HasIndex(o => o.ConversationId).IsUnique();

        // Payment
        modelBuilder.Entity<GoldenGems.Domain.Entities.Payment.Payment>()
            .HasOne(p => p.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<GoldenGems.Domain.Entities.Payment.Payment>()
            .Property(p => p.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<GoldenGems.Domain.Entities.Payment.Payment>()
            .Property(p => p.Method).HasConversion<string>();
        modelBuilder.Entity<GoldenGems.Domain.Entities.Payment.Payment>()
            .Property(p => p.Status).HasConversion<string>();
    }
}
