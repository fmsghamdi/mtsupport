using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Admin.Hotels;

[Authorize(Roles = "Admin,MabatSupport")]
public class CreateModel : PageModel
{
    private readonly MabatSupportDbContext _context;

    public CreateModel(MabatSupportDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Hotel Hotel { get; set; } = new();

    public SelectList Owners { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadOwnersAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadOwnersAsync();
            return Page();
        }

        Hotel.CreatedDate = DateTime.UtcNow;
        _context.Hotels.Add(Hotel);
        await _context.SaveChangesAsync();

        // If owner is set, update their user type
        if (!string.IsNullOrEmpty(Hotel.OwnerId))
        {
            var owner = await _context.Users.FindAsync(Hotel.OwnerId);
            if (owner != null)
            {
                owner.UserType = UserType.HotelOwner;
                owner.HotelId = Hotel.Id;
                await _context.SaveChangesAsync();
            }
        }

        TempData["SuccessMessage"] = $"تم إضافة الفندق {Hotel.Name} بنجاح";
        return RedirectToPage("Index");
    }

    private async Task LoadOwnersAsync()
    {
        // جلب فقط مالكي الفنادق (HotelOwner)
        var owners = await _context.Users
            .Where(u => u.UserType == UserType.HotelOwner)
            .OrderBy(u => u.FullName)
            .Select(u => new { u.Id, Name = u.FullName ?? u.Email })
            .ToListAsync();

        Owners = new SelectList(owners, "Id", "Name");
    }
}
