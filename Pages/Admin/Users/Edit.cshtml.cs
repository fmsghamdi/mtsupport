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
public class EditModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly MabatSupportDbContext _context;

    public EditModel(
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
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string? FullName { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        public string? Email { get; set; }

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

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            UserType = user.UserType,
            HotelId = user.HotelId,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive
        };

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

        var user = await _userManager.FindByIdAsync(Input.Id);
        if (user == null)
        {
            return NotFound();
        }

        // Update user properties
        user.FullName = Input.FullName;
        user.PhoneNumber = Input.PhoneNumber;
        user.IsActive = Input.IsActive;
        
        // Update hotel if applicable
        user.HotelId = (Input.UserType == UserType.HotelOwner || Input.UserType == UserType.HotelSupport)
            ? Input.HotelId : null;

        // Handle role change if user type changed
        if (user.UserType != Input.UserType)
        {
            // Remove old role
            var oldRoleName = user.UserType switch
            {
                UserType.Admin => "Admin",
                UserType.MabatSupport => "MabatSupport",
                UserType.HotelSupport => "HotelSupport",
                UserType.HotelOwner => "HotelOwner",
                _ => "User"
            };
            
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Add new role
            var newRoleName = Input.UserType switch
            {
                UserType.Admin => "Admin",
                UserType.MabatSupport => "MabatSupport",
                UserType.HotelSupport => "HotelSupport",
                UserType.HotelOwner => "HotelOwner",
                _ => "User"
            };

            if (!await _roleManager.RoleExistsAsync(newRoleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(newRoleName));
            }

            await _userManager.AddToRoleAsync(user, newRoleName);
            user.UserType = Input.UserType;
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "تم تحديث بيانات المستخدم بنجاح";
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
