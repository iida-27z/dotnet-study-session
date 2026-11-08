using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;

namespace StudySession.Domain.Participation;

/// <summary>
/// 勉強会の参加情報を永続化するリポジトリの契約です。
/// </summary>
public interface IStudySessionParticipationRepository
{
    /// <summary>
    /// 指定したセッションとユーザーの参加状況を取得します。
    /// </summary>
    /// <param name="sessionId">勉強会ID。</param>
    /// <param name="userId">ユーザーID。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    /// <returns>該当する参加情報。存在しない場合は null。</returns>
    Task<StudySessionParticipation?> FindBySessionAndUserIdAsync(StudySessionId sessionId, UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定したセッションの参加者一覧を取得します。
    /// </summary>
    /// <param name="sessionId">勉強会ID。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    /// <returns>対象セッションの参加者一覧。</returns>
    Task<List<StudySessionParticipation>> FindBySessionIdAsync(StudySessionId sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 参加情報を保存します。
    /// </summary>
    /// <param name="participation">保存対象の参加情報。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    Task SaveAsync(StudySessionParticipation participation, CancellationToken cancellationToken = default);
}
