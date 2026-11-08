namespace StudySession.Domain.StudySessions;

/// <summary>
/// 勉強会の永続化処理を担当するリポジトリの契約です。
/// </summary>
public interface IStudySessionRepository
{
    /// <summary>
    /// 新しい勉強会を保存します。
    /// </summary>
    /// <param name="studySession">保存対象の勉強会。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    Task AddAsync(StudySession studySession, CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定した勉強会IDに紐づく勉強会を取得します。
    /// </summary>
    /// <param name="studySessionId">検索対象の勉強会ID。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    /// <returns>該当する勉強会。存在しない場合は null。</returns>
    Task<StudySession?> FindByStudySessionIdAsync(StudySessionId studySessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定した開始日時以降の勉強会を開始日時の昇順で取得します。
    /// </summary>
    /// <param name="startTime">検索開始日時。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    /// <returns>条件に一致する勉強会の一覧。</returns>
    Task<List<StudySession>> FindByStartTimeOnOrAfterOrderByStartTimeAsync(DateTime startTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// 勉強会を保存します。
    /// </summary>
    /// <param name="studySession">保存対象の勉強会。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    Task SaveAsync(StudySession studySession, CancellationToken cancellationToken = default);
}
