using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;


namespace MabatSupportSystem.Pages.Tickets;

[Authorize]
public class CreateModel : PageModel
{
    private readonly MabatSupportDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

    public CreateModel(
        MabatSupportDbContext context, 
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<SharedResource> sharedLocalizer)
    {
        _context = context;
        _userManager = userManager;
        _sharedLocalizer = sharedLocalizer;
    }

    [BindProperty]
    public Ticket Ticket { get; set; } = new();

    public SelectList CategoryList { get; set; } = null!;
    public SelectList PriorityList { get; set; } = null!;
    
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // نزيل التحقق من هذه الحقول لأننا نملؤها تلقائياً
        ModelState.Remove("Ticket.UserId");
        ModelState.Remove("Ticket.CreatedDate");
        ModelState.Remove("Ticket.Status");
        ModelState.Remove("Ticket.UpdatedDate");
        ModelState.Remove("Ticket.UserName");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            ErrorMessage = "Validation failed: " + string.Join(", ", errors);
            await LoadListsAsync();
            return Page();
        }

        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // تعبئة البيانات
            Ticket.UserId = user.Id;

            // === التعديل هنا: حفظ الاسم المدخل، وإذا كان فارغاً نستخدم الإيميل ===
            if (string.IsNullOrEmpty(Ticket.UserName))
            {
                Ticket.UserName = user.UserName;
            }
            // ===================================================================

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
            await LoadListsAsync();
            return Page();
        }
    }

    private async Task LoadListsAsync()
    {
        // 1. ترجمة قائمة الأقسام (Categories)
        var dbCategories = await _context.TicketCategories.OrderBy(c => c.Id).ToListAsync();
        var localizedCategories = dbCategories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = GetLocalizedCategoryName(c.Id)
        }).ToList();
        CategoryList = new SelectList(localizedCategories, "Value", "Text");

        // 2. ترجمة قائمة الأولويات (Priorities)
        var priorities = Enum.GetValues(typeof(TicketPriority)).Cast<TicketPriority>().Select(p => new SelectListItem
        {
            Value = ((int)p).ToString(),
            Text = _sharedLocalizer[$"Priority_{p}"]
        }).ToList();
        PriorityList = new SelectList(priorities, "Value", "Text");
    }

    private string GetLocalizedCategoryName(int id)
    {
        return id switch
        {
            1 => _sharedLocalizer["Cat_CheckIn"],
            2 => _sharedLocalizer["Cat_Extension"],
            3 => _sharedLocalizer["Cat_Overnight"],
            4 => _sharedLocalizer["Cat_Hourly4"],
            5 => _sharedLocalizer["Cat_Hourly6"],
            6 => _sharedLocalizer["Cat_Payment"],
            7 => _sharedLocalizer["Cat_Cancel"],
            8 => _sharedLocalizer["Cat_General"],
            _ => "Unknown Category"
        };
    }
}
