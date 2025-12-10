using System.ComponentModel.DataAnnotations;

namespace MabatSupportSystem.Models;

/// <summary>
/// Represents a category for support tickets
/// Specific to Mabat hotel booking issues
/// </summary>
public class TicketCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Navigation property
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
