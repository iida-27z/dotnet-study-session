using System;

namespace StudySession.Web.Time;

public interface IDateTimeConverter
{
    /// <summary>
    /// 指定されたローカル日時を、設定されたタイムゾーンのオフセット付き DateTimeOffset に変換します。
    /// </summary>
    /// <param name="localDateTime">変換するローカル日時。DateTimeKind は Unspecified を前提とします。</param>
    /// <returns>変換後の DateTimeOffset。</returns>
    public DateTimeOffset ToDateTimeOffset(DateTime localDateTime);

    /// <summary>
    /// 指定された DateTimeOffset を、設定されたタイムゾーンのローカル日時に変換します。
    /// </summary>
    /// <param name="dateTimeOffset">変換する DateTimeOffset。</param>
    /// <returns>変換後のローカル日時。</returns>
    public DateTime ToLocalDateTime(DateTimeOffset dateTimeOffset);

}
