using StudySession.Domain.Users;

namespace StudySession.Domain.StudySessions;

/// <summary>
/// 勉強会を表すドメイン エンティティです。
/// </summary>
public sealed class StudySession
{
    /// <summary>
    /// <see cref="StudySession"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="sessionId">勉強会ID。</param>
    /// <param name="title">タイトル。</param>
    /// <param name="description">概要。</param>
    /// <param name="startTime">開始日時。</param>
    /// <param name="endTime">終了日時。</param>
    /// <param name="format">開催形式。</param>
    /// <param name="capacityOnsite">対面枠数。</param>
    /// <param name="capacityRemote">リモート枠数。</param>
    /// <param name="creatorUserId">主催者のユーザーID。</param>
    /// <param name="createdAt">作成日時。</param>
    /// <param name="updatedAt">更新日時。</param>
    /// <exception cref="ArgumentException">入力値が不正な場合。</exception>
    public StudySession(
        StudySessionId sessionId,
        string title,
        string description,
        DateTime startTime,
        DateTime endTime,
        StudySessionFormat format,
        int capacityOnsite,
        int capacityRemote,
        UserId creatorUserId,
        DateTime createdAt,
        DateTime updatedAt)
    {
        ArgumentNullException.ThrowIfNull(sessionId);
        ArgumentNullException.ThrowIfNull(creatorUserId);

        if (sessionId.Value == Guid.Empty)
        {
            throw new ArgumentException("Session ID must not be empty.", nameof(sessionId));
        }

        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be before end time.", nameof(startTime));
        }

        if (!Enum.IsDefined(typeof(StudySessionFormat), format))
        {
            throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported study session format.");
        }

        if (capacityOnsite < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacityOnsite), capacityOnsite, "Capacity must be greater than or equal to zero.");
        }

        if (capacityRemote < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacityRemote), capacityRemote, "Capacity must be greater than or equal to zero.");
        }

        SessionId = sessionId;
        Title = NormalizeRequiredString(title, 200, nameof(title));
        Description = NormalizeOptionalString(description, 2000, nameof(description));
        StartTime = startTime;
        EndTime = endTime;
        Format = format;
        CapacityOnsite = capacityOnsite;
        CapacityRemote = capacityRemote;
        CreatorUserId = creatorUserId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>
    /// 勉強会IDを取得します。
    /// </summary>
    public StudySessionId SessionId { get; }

    /// <summary>
    /// タイトルを取得します。
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// 概要を取得します。
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// 開始日時を取得します。
    /// </summary>
    public DateTime StartTime { get; }

    /// <summary>
    /// 終了日時を取得します。
    /// </summary>
    public DateTime EndTime { get; }

    /// <summary>
    /// 開催形式を取得します。
    /// </summary>
    public StudySessionFormat Format { get; }

    /// <summary>
    /// 対面枠数を取得します。
    /// </summary>
    public int CapacityOnsite { get; }

    /// <summary>
    /// リモート枠数を取得します。
    /// </summary>
    public int CapacityRemote { get; }

    /// <summary>
    /// 主催者のユーザーIDを取得します。
    /// </summary>
    public UserId CreatorUserId { get; }

    /// <summary>
    /// 作成日時を取得します。
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// 更新日時を取得します。
    /// </summary>
    public DateTime UpdatedAt { get; }

    private static string NormalizeRequiredString(string? value, int maxLength, string paramName)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Value is required.", paramName);
        }

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"Value must be <= {maxLength} characters.", paramName);
        }

        return normalized;
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
