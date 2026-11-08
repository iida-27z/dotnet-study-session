namespace StudySession.Domain.Participation;

/// <summary>
/// 出席状況を表す列挙型です。未回答、現地参加、リモート参加、欠席のいずれかを示します。
/// </summary>
public enum AttendanceStatus
{
    /// <summary>
    /// 未回答を示します。
    /// </summary>
    Unanswered = 0,
    /// <summary>
    /// 現地参加を示します。
    /// </summary>
    Onsite = 1,
    /// <summary>
    /// リモート参加を示します。
    /// </summary>
    Remote = 2,
    /// <summary>
    /// 欠席を示します。
    /// </summary>
    Absent = 3
}
