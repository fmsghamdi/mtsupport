using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Areas.Identity.Pages.Account.Manage
{
    public class TwoFactorAuthenticationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TwoFactorAuthenticationModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public bool Is2faEnabled { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            return Page();
        }
    }
}
