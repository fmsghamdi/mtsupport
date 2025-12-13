using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string? Email { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
            [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة")]
            public string NewEmail { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            Email = user.Email;
            Input.NewEmail = user.Email ?? string.Empty;
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                Email = user.Email;
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.NewEmail);
                if (!setEmailResult.Succeeded)
                {
                    StatusMessage = "حدث خطأ أثناء تغيير البريد الإلكتروني.";
                    return RedirectToPage();
                }
                await _userManager.SetUserNameAsync(user, Input.NewEmail);
            }

            StatusMessage = "تم تحديث البريد الإلكتروني بنجاح.";
            return RedirectToPage();
        }
    }
}
