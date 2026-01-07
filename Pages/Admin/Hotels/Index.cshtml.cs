using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Admin.Hotels;

[Authorize(Roles = "Admin,MabatSupport")]
public class IndexModel : PageModel
{
    private readonly MabatSupportDbContext _context;

    public IndexModel(MabatSupportDbContext context)
    {
        _context = context;
    }

    public List<Hotel> Hotels { get; set; } = new();

    public async Task OnGetAsync()
    {
        Hotels = await _context.Hotels
            .Include(h => h.SupportTeam)
            .Include(h => h.Owner)
            .OrderByDescending(h => h.CreatedDate)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        // Unlink users from this hotel
        var users = await _context.Users.Where(u => u.HotelId == id).ToListAsync();
        foreach (var user in users)
        {
            user.HotelId = null;
        }

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();

        TempData["Success"] = "تم حذف الفندق بنجاح";
        return RedirectToPage();
    }
}
