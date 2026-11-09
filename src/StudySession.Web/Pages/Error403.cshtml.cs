using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudySession.Web.Pages;

/// <summary>
/// Error page for 403 forbidden.
/// </summary>
[AllowAnonymous]
public sealed class Error403Model : PageModel
{
    /// <summary>
    /// Handles GET requests.
    /// </summary>
    public void OnGet()
    {
    }
}
