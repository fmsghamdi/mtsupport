using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MabatSupportSystem.Models;

namespace MabatSupportSystem.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public PersonalDataModel(
            UserManager<ApplicationUser> userManager,
            ILogger<PersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDownloadPersonalDataAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            _logger.LogInformation("User requested their personal data download.");

            var personalData = new Dictionary<string, string?>
            {
                { "Id", user.Id },
                { "UserName", user.UserName },
                { "Email", user.Email },
                { "FullName", user.FullName },
                { "PhoneNumber", user.PhoneNumber },
                { "EmailConfirmed", user.EmailConfirmed.ToString() },
                { "PhoneNumberConfirmed", user.PhoneNumberConfirmed.ToString() },
                { "TwoFactorEnabled", user.TwoFactorEnabled.ToString() }
            };

            Response.Headers.Append("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(
                JsonSerializer.SerializeToUtf8Bytes(personalData, new JsonSerializerOptions { WriteIndented = true }), 
                "application/json");
        }
    }
}
