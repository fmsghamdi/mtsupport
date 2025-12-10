using Microsoft.AspNetCore.Identity;

namespace MabatSupportSystem.Models;

/// <summary>
/// Application user extending Identity user
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    
    // Navigation property for user's tickets
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
