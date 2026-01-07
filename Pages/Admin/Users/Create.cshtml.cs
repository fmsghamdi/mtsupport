using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MabatSupportSystem.Data;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Pages.Admin.Users;

[Authorize(Roles = "Admin,MabatSupport")]
public class CreateModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly MabatSupportDbContext _context;

    public CreateModel(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        MabatSupportDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList Hotels { get; set; } = null!;

    public class InputModel
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, ErrorMessage = "كلمة المرور يجب أن تكون على الأقل {2} أحرف", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "نوع المستخدم")]
        public UserType UserType { get; set; }

        [Display(Name = "الفندق")]
        public int? HotelId { get; set; }

        [Phone(ErrorMessage = "رقم الهاتف غير صالح")]
        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadHotelsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadHotelsAsync();
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FullName = Input.FullName,
            PhoneNumber = Input.PhoneNumber,
            UserType = Input.UserType,
            HotelId = (Input.UserType == UserType.HotelOwner || Input.UserType == UserType.HotelSupport) 
                ? Input.HotelId : null,
            IsActive = Input.IsActive,
            CreatedDate = DateTime.UtcNow,
            EmailConfirmed = true // Auto-confirm for admin-created users
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            // Assign role based on UserType
            var roleName = Input.UserType switch
            {
                UserType.Admin => "Admin",
                UserType.MabatSupport => "MabatSupport",
                UserType.HotelSupport => "HotelSupport",
                UserType.HotelOwner => "HotelOwner",
                _ => "User"
            };

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);

            TempData["SuccessMessage"] = $"تم إنشاء المستخدم {Input.FullName} بنجاح";
            return RedirectToPage("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        await LoadHotelsAsync();
        return Page();
    }

    private async Task LoadHotelsAsync()
    {
        var hotels = await _context.Hotels
            .Where(h => h.IsActive)
            .OrderBy(h => h.Name)
            .ToListAsync();

        Hotels = new SelectList(hotels, "Id", "Name");
    }
}
