using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudySession.Web.Pages;

/// <summary>
/// Error page for 500 internal server error.
/// </summary>
[AllowAnonymous]
public sealed class Error500Model : PageModel
{
    /// <summary>
    /// Handles GET requests.
    /// </summary>
    public void OnGet()
    {
    }
}
