using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Admin;

[Authorize(Roles = "Admin,MabatSupport")]
public class IndexModel : PageModel
{
    private readonly MabatSupportDbContext _context;

    public IndexModel(MabatSupportDbContext context)
    {
        _context = context;
    }

    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int TotalUsers { get; set; }
    public int TotalHotels { get; set; }
    public Dictionary<UserType, int> UserTypeStats { get; set; } = new();
    public List<Ticket> RecentTickets { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Get statistics
        TotalTickets = await _context.Tickets.CountAsync();
        OpenTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Open || t.Status == TicketStatus.InProgress);
        TotalUsers = await _context.Users.CountAsync();
        TotalHotels = await _context.Hotels.CountAsync();

        // Get user type statistics
        var users = await _context.Users.ToListAsync();
        UserTypeStats = users
            .GroupBy(u => u.UserType)
            .ToDictionary(g => g.Key, g => g.Count());

        // Ensure all user types are in the dictionary
        foreach (UserType userType in Enum.GetValues(typeof(UserType)))
        {
            if (!UserTypeStats.ContainsKey(userType))
            {
                UserTypeStats[userType] = 0;
            }
        }

        // Get recent tickets
        RecentTickets = await _context.Tickets
            .Include(t => t.Category)
            .OrderByDescending(t => t.CreatedDate)
            .Take(10)
            .ToListAsync();

        return Page();
    }
}
