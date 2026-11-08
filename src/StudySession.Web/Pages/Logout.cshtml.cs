using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudySession.Web.Pages;

/// <summary>
/// Logout page model.
/// </summary>
[Authorize]
public sealed class LogoutModel : PageModel
{
    /// <summary>
    /// Signs out the current user.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Login");
    }

    /// <summary>
    /// Redirects GET requests to the login page.
    /// </summary>
    public IActionResult OnGet()
    {
        return RedirectToPage("/Login");
    }
}
