using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;

namespace StudySession.Domain.Participation;

/// <summary>
/// 勉強会への参加状況を表すドメイン エンティティです。
/// </summary>
public sealed class StudySessionParticipation
{
    /// <summary>
    /// <see cref="StudySessionParticipation"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="sessionId">対象の勉強会ID。</param>
    /// <param name="userId">参加ユーザーID。</param>
    /// <param name="attendanceStatus">出欠状況。</param>
    /// <param name="comment">コメント。</param>
    /// <param name="respondedAt">回答日時。</param>
    /// <param name="updatedAt">更新日時。</param>
    /// <param name="displayName">表示名。省略時はユーザーID文字列を利用する。</param>
    /// <param name="loginId">ログインID。省略時はユーザーID文字列を利用する。</param>
    public StudySessionParticipation(
        StudySessionId sessionId,
        UserId userId,
        AttendanceStatus attendanceStatus,
        string? comment,
        DateTime? respondedAt,
        DateTime updatedAt,
        string? displayName = null,
        string? loginId = null)
    {
        ArgumentNullException.ThrowIfNull(sessionId);
        ArgumentNullException.ThrowIfNull(userId);

        if (!Enum.IsDefined(typeof(AttendanceStatus), attendanceStatus))
        {
            throw new ArgumentOutOfRangeException(nameof(attendanceStatus), attendanceStatus, "Unsupported attendance status.");
        }

        SessionId = sessionId;
        UserId = userId;
        AttendanceStatus = attendanceStatus;
        Comment = NormalizeOptionalString(comment, 500, nameof(comment));
        RespondedAt = respondedAt;
        UpdatedAt = updatedAt;
        DisplayName = string.IsNullOrWhiteSpace(displayName)
            ? userId.ToString()
            : NormalizeOptionalString(displayName, 100, nameof(displayName));
        LoginId = string.IsNullOrWhiteSpace(loginId)
            ? userId.ToString()
            : NormalizeOptionalString(loginId, 64, nameof(loginId));
    }

    /// <summary>
    /// 勉強会IDを取得します。
    /// </summary>
    public StudySessionId SessionId { get; }

    /// <summary>
    /// 参加ユーザーIDを取得します。
    /// </summary>
    public UserId UserId { get; }

    /// <summary>
    /// 出欠状況を取得または設定します。
    /// </summary>
    public AttendanceStatus AttendanceStatus { get; private set; }

    /// <summary>
    /// コメントを取得または設定します。
    /// </summary>
    public string Comment { get; private set; }

    /// <summary>
    /// 回答日時を取得または設定します。
    /// </summary>
    public DateTime? RespondedAt { get; private set; }

    /// <summary>
    /// 更新日時を取得または設定します。
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// 表示名を取得または設定します。
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// ログインIDを取得します。
    /// </summary>
    public string LoginId { get; private set; }

    /// <summary>
    /// 出欠情報を更新します。
    /// </summary>
    /// <param name="attendanceStatus">更新後の出欠状況。</param>
    /// <param name="comment">更新後のコメント。</param>
    /// <param name="respondedAt">回答日時。</param>
    /// <param name="updatedAt">更新日時。</param>
    public void UpdateAttendance(AttendanceStatus attendanceStatus, string? comment, DateTime? respondedAt, DateTime? updatedAt = null)
    {
        if (!Enum.IsDefined(typeof(AttendanceStatus), attendanceStatus))
        {
            throw new ArgumentOutOfRangeException(nameof(attendanceStatus), attendanceStatus, "Unsupported attendance status.");
        }

        AttendanceStatus = attendanceStatus;
        Comment = NormalizeOptionalString(comment, 500, nameof(comment));
        RespondedAt = respondedAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    /// <summary>
    /// 指定した開催形式での参加可否を判定します。
    /// </summary>
    /// <param name="format">開催形式。</param>
    /// <returns>参加可能な場合は true。</returns>
    public bool IsAllowedFor(StudySessionFormat format)
    {
        return format switch
        {
            StudySessionFormat.OnsiteOnly => AttendanceStatus != AttendanceStatus.Remote,
            StudySessionFormat.RemoteOnly => AttendanceStatus != AttendanceStatus.Onsite,
            _ => true,
        };
    }

    /// <summary>
    /// 指定した開催形式で参加できるかどうかを判定します。
    /// </summary>
    /// <param name="format">開催形式。</param>
    /// <returns>参加可能な場合は true。</returns>
    public bool CanAttend(StudySessionFormat format)
    {
        return IsAllowedFor(format);
    }

    private static string NormalizeOptionalString(string? value, int maxLength, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"Value must be <= {maxLength} characters.", paramName);
        }

        return normalized;
    }
}
