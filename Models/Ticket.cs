using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MabatSupportSystem.Models;

/// <summary>
/// Represents a support ticket for the Mabat hotel booking system
/// </summary>
public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TicketStatus Status { get; set; } = TicketStatus.Open;

    [Required]
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? UserName { get; set; }

    /// <summary>
    /// Reference to the Mabat booking system - nullable for general inquiries
    /// </summary>
    [MaxLength(100)]
    public string? BookingReferenceId { get; set; }

    // Foreign key for Category
    [Required]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public TicketCategory? Category { get; set; }

    // Navigation property for responses
    public ICollection<TicketResponse> Responses { get; set; } = new List<TicketResponse>();
}
