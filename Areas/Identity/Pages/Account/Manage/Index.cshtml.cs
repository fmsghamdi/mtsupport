using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // تهيئة المتغيرات = string.Empty;
        public string Username { get; set; } = string.Empty;

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty; // تهيئة

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; } = string.Empty; // تهيئة
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName ?? string.Empty;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber ?? string.Empty,
                FullName = user.FullName ?? string.Empty
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            }

            // تحديث الاسم الكامل
            if (Input.FullName != user.FullName)
            {
                user.FullName = Input.FullName;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}