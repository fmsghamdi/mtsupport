using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Admin.Users;

[Authorize(Roles = "Admin,MabatSupport")]
public class IndexModel : PageModel
{
    private readonly MabatSupportDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(MabatSupportDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<ApplicationUser> Users { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? UserTypeFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Users
            .Include(u => u.Hotel)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            query = query.Where(u => 
                (u.FullName != null && u.FullName.Contains(SearchTerm)) ||
                (u.Email != null && u.Email.Contains(SearchTerm)) ||
                (u.UserName != null && u.UserName.Contains(SearchTerm)));
        }

        // User type filter
        if (!string.IsNullOrEmpty(UserTypeFilter) && Enum.TryParse<UserType>(UserTypeFilter, out var userType))
        {
            query = query.Where(u => u.UserType == userType);
        }

        // Status filter
        if (!string.IsNullOrEmpty(StatusFilter))
        {
            if (StatusFilter == "active")
                query = query.Where(u => u.IsActive);
            else if (StatusFilter == "inactive")
                query = query.Where(u => !u.IsActive);
        }

        Users = await query
            .OrderByDescending(u => u.CreatedDate)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Don't allow deleting yourself
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser?.Id == id)
        {
            TempData["Error"] = "لا يمكنك حذف حسابك الخاص";
            return RedirectToPage();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["Success"] = "تم حذف المستخدم بنجاح";
        }
        else
        {
            TempData["Error"] = "حدث خطأ أثناء حذف المستخدم";
        }

        return RedirectToPage();
    }
}
