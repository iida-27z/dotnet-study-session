namespace StudySession.Application.StudySessions;

/// <summary>
/// JST を前提としたブラウザ入力と UTC 保持を相互変換するヘルパーです。
/// サーバーのシステム クロックは <see cref="TimeProvider"/> で注入可能です。
/// </summary>
public sealed class StudySessionTimeZone
{
    private static readonly TimeZoneInfo JapanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
    private readonly TimeProvider _timeProvider;

    public StudySessionTimeZone(TimeProvider? timeProvider = null)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public DateTime UtcNow => _timeProvider.GetUtcNow().UtcDateTime;

    /// <summary>
    /// ブラウザ入力で送られてくる JST 時刻を UTC に変換します。
    /// </summary>
    public DateTime ToUtcFromBrowserJst(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        if (value.Kind == DateTimeKind.Local)
        {
            return value.ToUniversalTime();
        }

        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), JapanTimeZone);
    }

    /// <summary>
    /// UTC で保存された時刻をブラウザ表示用の JST に変換します。
    /// </summary>
    public DateTime ToDisplayJst(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(value, JapanTimeZone);
        }

        if (value.Kind == DateTimeKind.Local)
        {
            return TimeZoneInfo.ConvertTime(value, JapanTimeZone);
        }

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(value, DateTimeKind.Utc), JapanTimeZone);
    }
}
