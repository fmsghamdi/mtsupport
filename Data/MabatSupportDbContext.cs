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
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<TicketRating> TicketRatings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========== Configure ApplicationUser ==========
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.UserType)
                .HasConversion<int>();

            entity.Property(e => e.FullName)
                .HasMaxLength(200);

            entity.Property(e => e.ProfileImageUrl)
                .HasMaxLength(500);

            // Relationship with Hotel
            entity.HasOne(u => u.Hotel)
                .WithMany(h => h.SupportTeam)
                .HasForeignKey(u => u.HotelId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.UserType);
            entity.HasIndex(e => e.HotelId);
        });

        // ========== Configure Hotel ==========
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Address)
                .HasMaxLength(500);

            entity.Property(e => e.City)
                .HasMaxLength(100);

            entity.Property(e => e.Phone)
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .HasMaxLength(200);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Owner relationship
            entity.HasOne(h => h.Owner)
                .WithMany()
                .HasForeignKey(h => h.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
        });

        // ========== Configure Ticket ==========
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.Priority)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.UserName)
                .HasMaxLength(100);

            entity.Property(e => e.BookingReferenceId)
                .HasMaxLength(100);

            entity.Property(e => e.InternalNotes)
                .HasMaxLength(2000);

            // Relationship with Category
            entity.HasOne(t => t.Category)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with User (Creator)
            entity.HasOne(t => t.ApplicationUser)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Assigned User
            entity.HasOne(t => t.AssignedTo)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relationship with Hotel
            entity.HasOne(t => t.Hotel)
                .WithMany(h => h.Tickets)
                .HasForeignKey(t => t.HotelId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.AssignedToId);
            entity.HasIndex(e => e.HotelId);
            entity.HasIndex(e => e.BookingReferenceId);
            entity.HasIndex(e => e.CreatedDate);
        });

        // ========== Configure TicketCategory ==========
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

        // ========== Configure TicketResponse ==========
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

            // Relationship with Ticket
            entity.HasOne(r => r.Ticket)
                .WithMany(t => t.Responses)
                .HasForeignKey(r => r.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.TicketId);
            entity.HasIndex(e => e.CreatedDate);
        });

        // ========== Configure Notification ==========
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.Type)
                .HasConversion<int>();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.ActionUrl)
                .HasMaxLength(500);

            entity.Property(e => e.UserId)
                .IsRequired();

            // Relationship with User
            entity.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Ticket (optional)
            entity.HasOne(n => n.Ticket)
                .WithMany(t => t.Notifications)
                .HasForeignKey(n => n.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.CreatedDate);
        });

        // ========== Configure TicketRating ==========
        modelBuilder.Entity<TicketRating>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.Rating)
                .IsRequired();

            entity.Property(e => e.Comment)
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UserId)
                .IsRequired();

            // Relationship with Ticket (one-to-one)
            entity.HasOne(r => r.Ticket)
                .WithOne(t => t.Rating)
                .HasForeignKey<TicketRating>(r => r.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with User (who gave rating)
            entity.HasOne(r => r.User)
                .WithMany(u => u.GivenRatings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Support Agent (who received rating)
            entity.HasOne(r => r.SupportAgent)
                .WithMany(u => u.ReceivedRatings)
                .HasForeignKey(r => r.SupportAgentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.TicketId)
                .IsUnique(); // One rating per ticket

            entity.HasIndex(e => e.SupportAgentId);
        });

        // ========== Seed Data ==========
        
        // Seed initial categories
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
