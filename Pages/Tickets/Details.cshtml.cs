using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Tickets;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly MabatSupportDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DetailsModel(MabatSupportDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Ticket Ticket { get; set; } = default!;

    [BindProperty]
    public TicketResponse NewResponse { get; set; } = new();

    // خاصية لتغيير الحالة من الواجهة
    [BindProperty]
    public TicketStatus NewStatus { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var ticket = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Responses)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (ticket == null) return NotFound();

        // التعديل 1: السماح بالدخول إذا كان المستخدم هو صاحب التذكرة OR هو أدمن
        if (ticket.UserId != user.Id && !User.IsInRole("Admin")) 
        {
            return Forbid();
        }

        Ticket = ticket;
        NewStatus = ticket.Status; // تعيين الحالة الحالية
        return Page();
    }

    // دالة إضافة رد
    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) return NotFound();

        // حماية: التأكد من الصلاحية
        if (ticket.UserId != user.Id && !User.IsInRole("Admin")) return Forbid();

        if (!string.IsNullOrWhiteSpace(NewResponse.Message))
        {
            NewResponse.TicketId = ticket.Id;
            NewResponse.ResponderId = user.Id;
            NewResponse.ResponderName = User.IsInRole("Admin") ? "Admin Support" : user.UserName; // تمييز اسم الأدمن
            NewResponse.CreatedDate = DateTime.UtcNow;
            
            // التعديل 2: تحديد هل الرد من موظف أم لا
            NewResponse.IsStaffResponse = User.IsInRole("Admin");

            _context.TicketResponses.Add(NewResponse);
            
            // تحديث وقت آخر تعديل
            ticket.UpdatedDate = DateTime.UtcNow;

            // إذا رد الأدمن، نغير الحالة تلقائياً إلى "قيد التنفيذ" إذا كانت مفتوحة
            if (User.IsInRole("Admin") && ticket.Status == TicketStatus.Open)
            {
                ticket.Status = TicketStatus.InProgress;
            }
            
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id = id });
    }

    // دالة جديدة: تغيير حالة التذكرة (للأدمن فقط)
    public async Task<IActionResult> OnPostUpdateStatusAsync(int? id)
    {
        if (id == null) return NotFound();
        
        // التحقق أن المستخدم أدمن
        if (!User.IsInRole("Admin")) return Forbid();

        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        ticket.Status = NewStatus;
        ticket.UpdatedDate = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id = id });
    }
}