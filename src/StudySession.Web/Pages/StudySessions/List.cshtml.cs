using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudySession.Application.AttendanceSummary;
using StudySession.Domain.Users;

namespace StudySession.Web.Pages.StudySessions;

/// <summary>
/// Study session list page.
/// </summary>
[Authorize]
public sealed class ListModel : PageModel
{
    private readonly GetAttendanceSummaryUseCase _getStudySessionParticipationSummaryUseCase;

    public ListModel(GetAttendanceSummaryUseCase getStudySessionParticipationSummaryUseCase)
    {
        _getStudySessionParticipationSummaryUseCase = getStudySessionParticipationSummaryUseCase;
    }

    /// <summary>
    /// Gets the list of study session summaries shown on the page.
    /// </summary>
    public List<StudySessionParticipationSummaryDto> Sessions { get; private set; } = [];

    /// <summary>
    /// Handles GET requests.
    /// </summary>
    public async Task OnGetAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userIdValue))
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var userId = UserId.From(userIdValue);
        Sessions = await _getStudySessionParticipationSummaryUseCase.ExecuteAsync(DateTime.Today, userId);
    }
}
