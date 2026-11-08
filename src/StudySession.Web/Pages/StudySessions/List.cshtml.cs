using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudySession.Application.AttendanceSummary;
using StudySession.Application.StudySessions;
using StudySession.Domain.Users;
using StudySession.Web.Time;

namespace StudySession.Web.Pages.StudySessions;

/// <summary>
/// Study session list page.
/// </summary>
[Authorize]
public sealed class ListModel : PageModel
{
    private readonly GetAttendanceSummaryUseCase _getStudySessionParticipationSummaryUseCase;
    private readonly IDateTimeConverter _dateTimeConverter;
    private readonly TimeProvider _timeProvider;

    public ListModel(GetAttendanceSummaryUseCase getStudySessionParticipationSummaryUseCase, IDateTimeConverter dateTimeConverter, TimeProvider timeProvider)
    {
        _getStudySessionParticipationSummaryUseCase = getStudySessionParticipationSummaryUseCase ?? throw new ArgumentNullException(nameof(getStudySessionParticipationSummaryUseCase));
        _dateTimeConverter = dateTimeConverter ?? throw new ArgumentNullException(nameof(dateTimeConverter));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    public IDateTimeConverter TimeZone => _dateTimeConverter;

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
        var now = _timeProvider.GetUtcNow();
        var todayLocalDate = _dateTimeConverter.ToLocalDateTime(now).Date;
        var todayUtc = _dateTimeConverter.ToDateTimeOffset(todayLocalDate);
        Sessions = await _getStudySessionParticipationSummaryUseCase.ExecuteAsync(todayUtc, userId);
    }
}
