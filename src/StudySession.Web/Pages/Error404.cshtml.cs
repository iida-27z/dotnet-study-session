using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudySession.Web.Pages;

/// <summary>
/// Error page for 404 not found.
/// </summary>
[AllowAnonymous]
public sealed class Error404Model : PageModel
{
    /// <summary>
    /// Handles GET requests.
    /// </summary>
    public void OnGet()
    {
    }
}
