using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Tickets;

[Authorize]
public class IndexModel : PageModel
{
    private readonly MabatSupportDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(MabatSupportDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<Ticket> Tickets { get;set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public TicketStatus? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public TicketPriority? Priority { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        // نبدأ الاستعلام
        var query = _context.Tickets
            .Include(t => t.Category)
            .AsQueryable();

        // التعديل الذكي هنا:
        // إذا لم يكن مديراً، نفلتر حسب المستخدم.
        // أما المدير، فيتجاوز هذا الشرط ويرى كل شيء.
        if (!User.IsInRole("Admin"))
        {
            query = query.Where(t => t.UserId == user.Id);
        }

        // تطبيق البحث
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            query = query.Where(t => t.Title.Contains(SearchTerm) || 
                                     (t.BookingReferenceId != null && t.BookingReferenceId.Contains(SearchTerm)));
        }

        // تطبيق فلتر الحالة
        if (Status.HasValue)
        {
            query = query.Where(t => t.Status == Status.Value);
        }

        // تطبيق فلتر الأولوية
        if (Priority.HasValue)
        {
            query = query.Where(t => t.Priority == Priority.Value);
        }

        // الترتيب: الأحدث أولاً
        query = query.OrderByDescending(t => t.CreatedDate);

        Tickets = await query.ToListAsync();
    }
}