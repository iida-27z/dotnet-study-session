using System;
using StudySession.Domain.Users;

namespace StudySession.Application.AttendanceSummary;

public interface IStudySessionParticipationSummaryQueryService
{
    /// <summary>
    /// 指定された日付以降に開催される勉強会とその申し込み状況サマリーを取得します。
    /// </summary>
    /// <param name="fromDate">取得対象の開始日付</param>
    /// <param name="userId">対象のユーザーID</param>
    /// <returns>勉強会の参加状況サマリーのリスト</returns>
    Task<List<StudySessionParticipationSummaryDto>> GetParticipationSummaryAsync(DateTime fromDate, UserId userId, CancellationToken cancellationToken);

}
