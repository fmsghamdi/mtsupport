using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MabatSupportSystem.Pages;

public class SetLanguageModel : PageModel
{
    public IActionResult OnGet(string culture, string returnUrl = "/")
    {
        // Validate the culture
        if (string.IsNullOrEmpty(culture) || !IsValidCulture(culture))
        {
            culture = "en-US"; // Default fallback
        }

        // Set the culture cookie
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions 
            { 
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                Path = "/",
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Lax
            }
        );

        // Redirect back to the page the user came from
        return LocalRedirect(returnUrl);
    }

    private bool IsValidCulture(string culture)
    {
        var supportedCultures = new[] { "en-US", "ar-SA" };
        return supportedCultures.Contains(culture);
    }
}
