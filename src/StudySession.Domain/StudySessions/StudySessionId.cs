namespace StudySession.Domain.StudySessions;

/// <summary>
/// 勉強会IDを表す値オブジェクトです。
/// </summary>
public record StudySessionId
{
    /// <summary>
    /// ID の内部値を取得します。
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// 新しい勉強会IDを生成します。
    /// </summary>
    /// <returns>生成された勉強会ID。</returns>
    public static StudySessionId New()
    {
        return From(Guid.NewGuid());
    }

    /// <summary>
    /// 指定した GUID から勉強会IDを生成します。
    /// </summary>
    /// <param name="value">GUID 値。</param>
    /// <returns>生成された勉強会ID。</returns>
    public static StudySessionId From(Guid value)
    {
        return new StudySessionId(value);
    }

    /// <summary>
    /// GUID への暗黙変換を提供します。
    /// </summary>
    /// <param name="studySessionId">変換元の勉強会ID。</param>
    public static implicit operator Guid(StudySessionId studySessionId)
    {
        ArgumentNullException.ThrowIfNull(studySessionId);

        return studySessionId.Value;
    }

    /// <summary>
    /// GUID 文字列を返します。
    /// </summary>
    /// <returns>GUID の文字列表現。</returns>
    public override string ToString()
    {
        return Value.ToString();
    }

    private StudySessionId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Study Session ID must not be empty.", nameof(value));
        }

        Value = value;
    }
}
