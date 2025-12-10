using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Tickets;

public class IndexModel : PageModel
{
    private readonly MabatSupportDbContext _context;

    public IndexModel(MabatSupportDbContext context)
    {
        _context = context;
    }

    public List<Ticket> Tickets { get; set; } = new();
    public int? StatusFilter { get; set; }
    public int? PriorityFilter { get; set; }
    public string? SearchString { get; set; }

    public async Task OnGetAsync(int? statusFilter, int? priorityFilter, string? searchString)
    {
        StatusFilter = statusFilter;
        PriorityFilter = priorityFilter;
        SearchString = searchString;

        // Build query
        var query = _context.Tickets
            .Include(t => t.Category)
            .AsQueryable();

        // Apply filters
        if (statusFilter.HasValue)
        {
            query = query.Where(t => (int)t.Status == statusFilter.Value);
        }

        if (priorityFilter.HasValue)
        {
            query = query.Where(t => (int)t.Priority == priorityFilter.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(t => 
                t.Title.Contains(searchString) || 
                (t.BookingReferenceId != null && t.BookingReferenceId.Contains(searchString)));
        }

        // Order by created date (newest first)
        Tickets = await query
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();
    }
}
