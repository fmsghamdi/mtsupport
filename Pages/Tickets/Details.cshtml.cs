using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Tickets;

public class DetailsModel : PageModel
{
    private readonly MabatSupportDbContext _context;

    public DetailsModel(MabatSupportDbContext context)
    {
        _context = context;
    }

    public Ticket? Ticket { get; set; }
    public List<TicketResponse> Responses { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Ticket = await _context.Tickets
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (Ticket == null)
        {
            return NotFound();
        }

        // Load responses ordered by creation date
        Responses = await _context.TicketResponses
            .Where(r => r.TicketId == id)
            .OrderBy(r => r.CreatedDate)
            .ToListAsync();

        return Page();
    }

    /// <summary>
    /// Handler for updating ticket status (Admin workflow)
    /// </summary>
    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, int newStatus)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
        {
            return NotFound();
        }

        try
        {
            // Update the status
            ticket.Status = (TicketStatus)newStatus;
            ticket.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Ticket status updated to {ticket.Status}";
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to update status: {ex.Message}";
            return RedirectToPage(new { id });
        }
    }

    /// <summary>
    /// Handler for adding a response/reply to the ticket
    /// </summary>
    public async Task<IActionResult> OnPostAddResponseAsync(int id, string message, bool isStaffResponse = false)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            TempData["ErrorMessage"] = "Message cannot be empty";
            return RedirectToPage(new { id });
        }

        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
        {
            return NotFound();
        }

        try
        {
            // Create new response
            var response = new TicketResponse
            {
                TicketId = id,
                Message = message.Trim(),
                ResponderId = isStaffResponse ? "STAFF_ADMIN" : "GUEST_USER",
                ResponderName = isStaffResponse ? "Support Agent" : ticket.UserName ?? "Guest",
                IsStaffResponse = isStaffResponse,
                CreatedDate = DateTime.UtcNow
            };

            _context.TicketResponses.Add(response);
            
            // Update ticket's last updated timestamp
            ticket.UpdatedDate = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reply posted successfully";
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to post reply: {ex.Message}";
            return RedirectToPage(new { id });
        }
    }
}
