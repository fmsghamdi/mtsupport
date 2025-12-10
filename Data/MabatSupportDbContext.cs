using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Data;

/// <summary>
/// Database context for the Mabat Support Ticket System
/// Configured for PostgreSQL in production with SQLite support for local development
/// Now includes ASP.NET Core Identity
/// </summary>
public class MabatSupportDbContext : IdentityDbContext<ApplicationUser>
{
    public MabatSupportDbContext(DbContextOptions<MabatSupportDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketCategory> TicketCategories { get; set; }
    public DbSet<TicketResponse> TicketResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Ticket entity
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // PostgreSQL-specific: Use identity column for auto-increment
            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(2000);

            // PostgreSQL-specific: Store enums as integers
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.Priority)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Works for both PostgreSQL and SQLite

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.UserName)
                .HasMaxLength(100);

            entity.Property(e => e.BookingReferenceId)
                .HasMaxLength(100);

            // Configure relationship with Category
            entity.HasOne(t => t.Category)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for better query performance
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.BookingReferenceId);
            entity.HasIndex(e => e.CreatedDate);
        });

        // Configure TicketCategory entity
        modelBuilder.Entity<TicketCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.HasIndex(e => e.Name)
                .IsUnique();
        });

        // Configure TicketResponse entity
        modelBuilder.Entity<TicketResponse>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.ResponderId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.ResponderName)
                .HasMaxLength(100);

            entity.Property(e => e.IsStaffResponse)
                .IsRequired()
                .HasDefaultValue(false);

            // Configure relationship with Ticket
            entity.HasOne(r => r.Ticket)
                .WithMany(t => t.Responses)
                .HasForeignKey(r => r.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for better query performance
            entity.HasIndex(e => e.TicketId);
            entity.HasIndex(e => e.CreatedDate);
        });

        // Seed initial categories specific to Mabat hotel booking system
        modelBuilder.Entity<TicketCategory>().HasData(
            new TicketCategory
            {
                Id = 1,
                Name = "Check-in Issue",
                Description = "Problems related to check-in process or room access"
            },
            new TicketCategory
            {
                Id = 2,
                Name = "Extension Request (4h to 6h)",
                Description = "Request to extend hourly booking from 4 hours to 6 hours"
            },
            new TicketCategory
            {
                Id = 3,
                Name = "Overnight Booking Issue",
                Description = "Issues with full day bookings (8:00 PM to 12:00 PM next day)"
            },
            new TicketCategory
            {
                Id = 4,
                Name = "Hourly Booking (4h) Issue",
                Description = "Problems with 4-hour time slot bookings"
            },
            new TicketCategory
            {
                Id = 5,
                Name = "Hourly Booking (6h) Issue",
                Description = "Problems with 6-hour time slot bookings"
            },
            new TicketCategory
            {
                Id = 6,
                Name = "Payment & Billing",
                Description = "Payment issues, refunds, or billing questions"
            },
            new TicketCategory
            {
                Id = 7,
                Name = "Cancellation & Refund",
                Description = "Booking cancellation and refund requests"
            },
            new TicketCategory
            {
                Id = 8,
                Name = "General Inquiry",
                Description = "General questions about the booking system"
            }
        );
    }
}
