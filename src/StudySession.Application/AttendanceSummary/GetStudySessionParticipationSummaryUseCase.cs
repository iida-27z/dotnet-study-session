using StudySession.Domain.Users;

namespace StudySession.Application.AttendanceSummary;

/// <summary>
/// 勉強会の参加状況サマリーを取得するユースケースです。
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GetAttendanceSummaryUseCase"/> class.
/// </remarks>
public sealed class GetAttendanceSummaryUseCase(IStudySessionParticipationSummaryQueryService queryService)
{
    private readonly IStudySessionParticipationSummaryQueryService _queryService = queryService;

    /// <summary>
    /// 指定日の以降に開催される勉強会の参加状況サマリーを取得します。
    /// </summary>
    public Task<List<StudySessionParticipationSummaryDto>> ExecuteAsync(DateTime fromDate, UserId userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        return _queryService.GetParticipationSummaryAsync(fromDate, userId, cancellationToken);
    }
}
