using System;

namespace StudySession.Web.Time;

/// <summary>
/// タイムゾーン設定を表します。アプリケーションで使用するデフォルトのタイムゾーンIDを保持します。
/// </summary>
public class TimeZoneSettings
{
    /// <summary>
    /// 設定セクションの名前を表します。
    /// </summary>
    public const string SectionName = "TimeZone";
    /// <summary>
    /// デフォルトのタイムゾーンIDを取得または設定します。
    /// </summary>
    public string DefaultTimeZoneId { get; init; } = "UTC";
}
