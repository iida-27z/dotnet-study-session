using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudySession.Web.Pages.StudySessions;

/// <summary>
/// Study session list page.
/// </summary>
[Authorize]
public sealed class ListModel : PageModel
{
    /// <summary>
    /// Handles GET requests.
    /// </summary>
    public void OnGet()
    {
    }
}
