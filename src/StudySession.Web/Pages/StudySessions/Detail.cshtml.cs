using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudySession.Domain.Participation;
using StudySession.Domain.StudySessions;
using StudySessionEntity = StudySession.Domain.StudySessions.StudySession;

namespace StudySession.Web.Pages.StudySessions;

/// <summary>
/// 勉強会詳細画面のモデルです。
/// </summary>
[Authorize]
public sealed class DetailModel : PageModel
{
    private readonly IStudySessionRepository _studySessionRepository;

    public DetailModel(IStudySessionRepository studySessionRepository)
    {
        _studySessionRepository = studySessionRepository;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = string.Empty;

    [BindProperty]
    public string AttendanceStatusValue { get; set; } = AttendanceStatus.Unanswered.ToString();

    [BindProperty]
    public string? Comment { get; set; }

    public StudySessionEntity? Session { get; private set; }

    public List<ParticipantRow> Participants { get; private set; } = [];

    public bool IsAttendanceOpen => Session is not null && DateTime.UtcNow <= GetDeadline(Session);

    public string DeadlineDisplay => Session is null ? string.Empty : GetDeadline(Session).ToString("yyyy/MM/dd HH:mm");

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var session = await LoadSessionAsync(cancellationToken);
        if (session is null)
        {
            return NotFound();
        }

        Session = session;
        Participants = BuildParticipants(session);
        AttendanceStatusValue = AttendanceStatus.Unanswered.ToString();
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
        Participants = BuildParticipants(session);

        if (!TryParseAttendanceStatus(AttendanceStatusValue, out var attendanceStatus))
        {
            ModelState.AddModelError(nameof(AttendanceStatusValue), "出欠ステータスが正しくありません。");
        }

        if (!string.IsNullOrWhiteSpace(Comment) && Comment.Length > 500)
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
            return Page();
        }

        return RedirectToPage("/StudySessions/List");
    }

    private async Task<StudySessionEntity?> LoadSessionAsync(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(Id, out var sessionIdGuid))
        {
            return null;
        }

        return await _studySessionRepository.FindByStudySessionIdAsync(StudySessionId.From(sessionIdGuid), cancellationToken);
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

    private static DateTime GetDeadline(StudySessionEntity session)
    {
        return session.StartTime.ToUniversalTime().Date.AddDays(-1).AddHours(18);
    }

    private static List<ParticipantRow> BuildParticipants(StudySessionEntity session)
    {
        var participantRows = new List<ParticipantRow>
        {
            new("USR001", "開発 太郎", AttendanceStatus.Onsite, "実機セッティングのお手伝い可能です"),
            new("USR005", "デザイン 美子", AttendanceStatus.Onsite, "業務都合のため16:30頃から途中参加します"),
            new("USR012", "運用 三郎", AttendanceStatus.Absent, "緊急メンテナンス作業のため欠席します"),
        };

        if (session.Format == StudySessionFormat.RemoteOnly)
        {
            participantRows[0] = new ParticipantRow("USR001", "開発 太郎", AttendanceStatus.Remote, "リモートで参加します");
            participantRows[1] = new ParticipantRow("USR005", "デザイン 美子", AttendanceStatus.Remote, "オンラインで参加予定です");
        }

        return participantRows;
    }

    public sealed record ParticipantRow(string UserId, string DisplayName, AttendanceStatus Status, string Comment);
}
