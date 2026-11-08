using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudySession.Application.StudySessions;
using StudySession.Domain.Participation;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;
using StudySessionEntity = StudySession.Domain.StudySessions.StudySession;

namespace StudySession.Web.Pages.StudySessions;

/// <summary>
/// 勉強会詳細画面のモデルです。
/// </summary>
[Authorize]
public sealed class DetailModel : PageModel
{
    private readonly IStudySessionRepository _studySessionRepository;
    private readonly IStudySessionParticipationRepository _studySessionParticipationRepository;
    private readonly StudySessionTimeZone _studySessionTimeZone;

    public DetailModel(
        IStudySessionRepository studySessionRepository,
        IStudySessionParticipationRepository studySessionParticipationRepository,
        StudySessionTimeZone studySessionTimeZone)
    {
        _studySessionRepository = studySessionRepository ?? throw new ArgumentNullException(nameof(studySessionRepository));
        _studySessionParticipationRepository = studySessionParticipationRepository ?? throw new ArgumentNullException(nameof(studySessionParticipationRepository));
        _studySessionTimeZone = studySessionTimeZone ?? throw new ArgumentNullException(nameof(studySessionTimeZone));
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = string.Empty;

    [BindProperty]
    public string AttendanceStatusValue { get; set; } = AttendanceStatus.Unanswered.ToString();

    [BindProperty]
    public string? Comment { get; set; }

    public StudySessionEntity? Session { get; private set; }

    public List<ParticipantRow> Participants { get; private set; } = [];

    public StudySessionTimeZone TimeZone => _studySessionTimeZone;

    public bool IsAttendanceOpen => Session is not null && _studySessionTimeZone.UtcNow <= GetDeadline(Session);

    public string DeadlineDisplay => Session is null ? string.Empty : _studySessionTimeZone.ToDisplayJst(GetDeadline(Session)).ToString("yyyy/MM/dd HH:mm");

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var session = await LoadSessionAsync(cancellationToken);
        if (session is null)
        {
            return NotFound();
        }

        Session = session;
        Participants = await LoadParticipantsAsync(session, cancellationToken);

        var currentUserId = GetCurrentUserId();
        if (currentUserId is not null)
        {
            var participation = await _studySessionParticipationRepository.FindBySessionAndUserIdAsync(session.SessionId, currentUserId, cancellationToken);
            if (participation is not null)
            {
                AttendanceStatusValue = participation.AttendanceStatus.ToString();
                Comment = participation.Comment;
            }
            else
            {
                AttendanceStatusValue = AttendanceStatus.Unanswered.ToString();
                Comment = string.Empty;
            }
        }
        else
        {
            AttendanceStatusValue = AttendanceStatus.Unanswered.ToString();
            Comment = string.Empty;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var session = await LoadSessionAsync(cancellationToken);
        if (session is null)
        {
            return NotFound();
        }

        Session = session;
        Participants = await LoadParticipantsAsync(session, cancellationToken);

        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Unauthorized();
        }

        if (!TryParseAttendanceStatus(AttendanceStatusValue, out var attendanceStatus))
        {
            ModelState.AddModelError(nameof(AttendanceStatusValue), "出欠ステータスが正しくありません。");
        }

        var normalizedComment = string.IsNullOrWhiteSpace(Comment) ? string.Empty : Comment.Trim();
        if (normalizedComment.Length > 500)
        {
            ModelState.AddModelError(nameof(Comment), "コメントは500文字以内で入力してください。");
        }

        if (Session.Format == StudySessionFormat.OnsiteOnly && attendanceStatus == AttendanceStatus.Remote)
        {
            ModelState.AddModelError(nameof(AttendanceStatusValue), "対面のみの開催ではリモート出席を選択できません。");
        }

        if (Session.Format == StudySessionFormat.RemoteOnly && attendanceStatus == AttendanceStatus.Onsite)
        {
            ModelState.AddModelError(nameof(AttendanceStatusValue), "リモートのみの開催では対面出席を選択できません。");
        }

        if (!IsAttendanceOpen)
        {
            ModelState.AddModelError(nameof(AttendanceStatusValue), "出欠変更の締切を過ぎています。開催前日の18:00までに更新してください。");
        }

        if (!ModelState.IsValid)
        {
            Comment = normalizedComment;
            return Page();
        }

        var now = _studySessionTimeZone.UtcNow;
        var participation = await _studySessionParticipationRepository.FindBySessionAndUserIdAsync(Session.SessionId, currentUserId, cancellationToken)
            ?? new StudySessionParticipation(Session.SessionId, currentUserId, attendanceStatus, normalizedComment, now, now);

        participation.UpdateAttendance(attendanceStatus, normalizedComment, now, now);
        await _studySessionParticipationRepository.SaveAsync(participation, cancellationToken);

        return RedirectToPage("/StudySessions/List");
    }

    private async Task<List<ParticipantRow>> LoadParticipantsAsync(StudySessionEntity session, CancellationToken cancellationToken)
    {
        var participants = await _studySessionParticipationRepository.FindBySessionIdAsync(session.SessionId, cancellationToken);
        if (participants.Count == 0)
        {
            return BuildParticipants(session);
        }

        return participants
            .Select(participant => new ParticipantRow(
                participant.LoginId,
                string.IsNullOrWhiteSpace(participant.DisplayName) ? participant.LoginId : participant.DisplayName,
                participant.AttendanceStatus,
                participant.Comment))
            .ToList();
    }

    private async Task<StudySessionEntity?> LoadSessionAsync(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(Id, out var sessionIdGuid))
        {
            return null;
        }

        return await _studySessionRepository.FindByStudySessionIdAsync(StudySessionId.From(sessionIdGuid), cancellationToken);
    }

    private UserId? GetCurrentUserId()
    {
        var principal = User;
        if (principal is null || principal.Identity is null || !principal.Identity.IsAuthenticated)
        {
            return null;
        }

        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userIdValue))
        {
            return null;
        }

        return UserId.From(userIdValue);
    }

    private static bool TryParseAttendanceStatus(string? value, out AttendanceStatus status)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            status = AttendanceStatus.Unanswered;
            return false;
        }

        return Enum.TryParse(value, true, out status)
            && Enum.IsDefined(typeof(AttendanceStatus), status);
    }

    private DateTime GetDeadline(StudySessionEntity session)
    {
        var deadlineInJst = _studySessionTimeZone.ToDisplayJst(session.StartTime).Date.AddDays(-1).AddHours(18);
        return _studySessionTimeZone.ToUtcFromBrowserJst(deadlineInJst);
    }

    private static List<ParticipantRow> BuildParticipants(StudySessionEntity session)
    {
        var participantRows = new List<ParticipantRow>
        {
            new("dev", "開発 太郎", AttendanceStatus.Onsite, "実機セッティングのお手伝い可能です"),
            new("designer", "デザイン 美子", AttendanceStatus.Onsite, "業務都合のため16:30頃から途中参加します"),
            new("ops", "運用 三郎", AttendanceStatus.Absent, "緊急メンテナンス作業のため欠席します"),
        };

        if (session.Format == StudySessionFormat.RemoteOnly)
        {
            participantRows[0] = new ParticipantRow("dev", "開発 太郎", AttendanceStatus.Remote, "リモートで参加します");
            participantRows[1] = new ParticipantRow("designer", "デザイン 美子", AttendanceStatus.Remote, "オンラインで参加予定です");
        }

        return participantRows;
    }

    public sealed record ParticipantRow(string LoginId, string DisplayName, AttendanceStatus Status, string Comment)
    {
        public string UserId => LoginId;
    }
}
