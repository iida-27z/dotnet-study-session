using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudySession.Application.AttendanceSummary;
using StudySession.Application.StudySessions;
using StudySession.Domain.Users;

namespace StudySession.Web.Pages.StudySessions;

/// <summary>
/// Study session list page.
/// </summary>
[Authorize]
public sealed class ListModel : PageModel
{
    private readonly GetAttendanceSummaryUseCase _getStudySessionParticipationSummaryUseCase;
    private readonly StudySessionTimeZone _studySessionTimeZone;

    public ListModel(GetAttendanceSummaryUseCase getStudySessionParticipationSummaryUseCase, StudySessionTimeZone studySessionTimeZone)
    {
        _getStudySessionParticipationSummaryUseCase = getStudySessionParticipationSummaryUseCase ?? throw new ArgumentNullException(nameof(getStudySessionParticipationSummaryUseCase));
        _studySessionTimeZone = studySessionTimeZone ?? throw new ArgumentNullException(nameof(studySessionTimeZone));
    }

    public StudySessionTimeZone TimeZone => _studySessionTimeZone;

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
        var todayUtc = _studySessionTimeZone.UtcNow.Date;
        Sessions = await _getStudySessionParticipationSummaryUseCase.ExecuteAsync(todayUtc, userId);
    }
}
