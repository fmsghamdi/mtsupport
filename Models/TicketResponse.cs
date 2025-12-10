using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MabatSupportSystem.Models;

/// <summary>
/// Represents a response/conversation message in a support ticket
/// </summary>
public class TicketResponse
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(100)]
    public string ResponderId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ResponderName { get; set; }

    [Required]
    public bool IsStaffResponse { get; set; } = false;

    // Foreign key for Ticket
    [Required]
    public int TicketId { get; set; }

    [ForeignKey(nameof(TicketId))]
    public Ticket? Ticket { get; set; }
}
