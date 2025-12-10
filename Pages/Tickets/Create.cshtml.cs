using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Tickets;

// Temporarily removing [Authorize] to test without login
public class CreateModel : PageModel
{
    private readonly MabatSupportDbContext _context;

    public CreateModel(MabatSupportDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Ticket Ticket { get; set; } = new();

    public SelectList Categories { get; set; } = null!;
    
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadCategoriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Remove UserId and CreatedDate from ModelState validation since we set them programmatically
        ModelState.Remove("Ticket.UserId");
        ModelState.Remove("Ticket.CreatedDate");
        ModelState.Remove("Ticket.Status");
        ModelState.Remove("Ticket.UpdatedDate");
        
        // Log validation errors for debugging
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            ErrorMessage = "Validation failed: " + string.Join(", ", errors);
            await LoadCategoriesAsync();
            return Page();
        }

        try
        {
            // Set system fields BEFORE saving
            Ticket.UserId = $"GUEST_{Guid.NewGuid().ToString()[..8]}"; // Temporary until Identity is fully integrated
            Ticket.CreatedDate = DateTime.UtcNow;
            Ticket.Status = TicketStatus.Open;

            _context.Tickets.Add(Ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Ticket #{Ticket.Id} created successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save ticket: {ex.Message}";
            await LoadCategoriesAsync();
            return Page();
        }
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _context.TicketCategories
            .OrderBy(c => c.Name)
            .ToListAsync();
        
        Categories = new SelectList(categories, "Id", "Name");
    }
}
