using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
            [DataType(DataType.Password)]
            public string OldPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
            [StringLength(100, ErrorMessage = "يجب أن تكون كلمة المرور {0} حرفاً على الأقل", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "كلمتا المرور غير متطابقتين")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "تم تغيير كلمة المرور بنجاح.";

            return RedirectToPage();
        }
    }
}
