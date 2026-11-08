namespace StudySession.Domain.StudySessions;

/// <summary>
/// 勉強会の開催形式を表します。
/// </summary>
public enum StudySessionFormat
{
    /// <summary>
    /// 対面とリモートを併用する形式です。
    /// </summary>
    Hybrid = 0,

    /// <summary>
    /// 対面のみで開催する形式です。
    /// </summary>
    OnsiteOnly = 1,

    /// <summary>
    /// リモートのみで開催する形式です。
    /// </summary>
    RemoteOnly = 2
}
