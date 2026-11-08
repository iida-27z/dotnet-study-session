using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudySession.Web.Pages.StudySessions;

/// <summary>
/// Study session creation page.
/// </summary>
[Authorize]
public sealed class CreateModel : PageModel
{
    /// <summary>
    /// Handles GET requests.
    /// </summary>
    public void OnGet()
    {
    }
}
