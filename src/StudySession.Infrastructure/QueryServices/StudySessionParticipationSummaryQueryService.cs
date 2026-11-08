using Dapper;
using StudySession.Application.AttendanceSummary;
using StudySession.Domain.Participation;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;
using StudySession.Infrastructure.Data;

namespace StudySession.Infrastructure.QueryServices;

/// <summary>
/// 勉強会一覧画面で表示する参加状況サマリーをデータベースから取得します。
/// </summary>
public sealed class StudySessionParticipationSummaryQueryService : IStudySessionParticipationSummaryQueryService
{
    private readonly IStudySessionDbConnectionFactory _connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="StudySessionParticipationSummaryQueryService"/> class.
    /// </summary>
    public StudySessionParticipationSummaryQueryService(IStudySessionDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc/>
    public async Task<List<StudySessionParticipationSummaryDto>> GetParticipationSummaryAsync(DateTime fromDate, UserId userId, CancellationToken cancellationToken)
    {
        const string sql = """
            WITH participation_counts AS (
                SELECT
                    session_id,
                    COUNT(*) FILTER (WHERE attendance_status = 'Onsite') AS onsite_count,
                    COUNT(*) FILTER (WHERE attendance_status = 'Remote') AS remote_count
                FROM session_participations
                GROUP BY session_id
            ),
            user_attendance AS (
                SELECT
                    session_id,
                    attendance_status
                FROM session_participations
                WHERE user_id = @UserId
            )
            SELECT
                s.session_id AS SessionId,
                s.starts_at AS StartTime,
                s.title AS Title,
                s.format AS Format,
                s.capacity_onsite AS CapacityOnsite,
                COALESCE(pc.onsite_count, 0) AS ParticipantsCountOnsite,
                s.capacity_remote AS CapacityRemote,
                COALESCE(pc.remote_count, 0) AS ParticipantsCountRemote,
                CAST(@UserId AS uuid) AS UserId,
                CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM user_attendance ua
                        WHERE ua.session_id = s.session_id AND ua.attendance_status = 'Absent'
                    ) THEN 'Absent'
                    WHEN EXISTS (
                        SELECT 1
                        FROM user_attendance ua
                        WHERE ua.session_id = s.session_id AND ua.attendance_status = 'Onsite'
                    ) THEN 'Onsite'
                    WHEN EXISTS (
                        SELECT 1
                        FROM user_attendance ua
                        WHERE ua.session_id = s.session_id AND ua.attendance_status = 'Remote'
                    ) THEN 'Remote'
                    ELSE 'Unanswered'
                END AS UserAttendanceStatus
            FROM study_sessions s
            LEFT JOIN participation_counts pc ON pc.session_id = s.session_id
            WHERE s.starts_at >= @FromDate
            ORDER BY s.starts_at, s.title
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<StudySessionParticipationSummaryRow>(
            sql,
            new { FromDate = fromDate, UserId = userId.Value }
        );

        return rows.Select(MapToDto).ToList();
    }

    private static StudySessionParticipationSummaryDto MapToDto(StudySessionParticipationSummaryRow row)
    {
        if (!Enum.TryParse<StudySessionFormat>(row.Format, out var format))
        {
            throw new InvalidOperationException($"Unsupported format value '{row.Format}'.");
        }

        var userAttendanceStatus = ParseAttendanceStatus(row.UserAttendanceStatus);
        var attendingOnsite = userAttendanceStatus == AttendanceStatus.Onsite
            ? AttendanceStatus.Onsite
            : userAttendanceStatus == AttendanceStatus.Absent
                ? AttendanceStatus.Absent
                : AttendanceStatus.Unanswered;

        var attendingRemote = userAttendanceStatus == AttendanceStatus.Remote
            ? AttendanceStatus.Remote
            : userAttendanceStatus == AttendanceStatus.Absent
                ? AttendanceStatus.Absent
                : AttendanceStatus.Unanswered;

        return new StudySessionParticipationSummaryDto(
            StudySessionId.From(row.SessionId),
            row.StartTime,
            row.Title,
            format,
            row.CapacityOnsite,
            Convert.ToInt32(row.ParticipantsCountOnsite),
            row.CapacityRemote,
            Convert.ToInt32(row.ParticipantsCountRemote),
            UserId.From(row.UserId),
            attendingOnsite,
            attendingRemote);
    }

    private static AttendanceStatus ParseAttendanceStatus(string attendanceStatus)
    {
        if (Enum.TryParse<AttendanceStatus>(attendanceStatus, out var status))
        {
            return status;
        }

        return AttendanceStatus.Unanswered;
    }

    private sealed class StudySessionParticipationSummaryRow
    {
        public Guid SessionId { get; set; }

        public DateTime StartTime { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Format { get; set; } = string.Empty;

        public int CapacityOnsite { get; set; }

        public long ParticipantsCountOnsite { get; set; }

        public int CapacityRemote { get; set; }

        public long ParticipantsCountRemote { get; set; }

        public Guid UserId { get; set; }

        public string UserAttendanceStatus { get; set; } = string.Empty;
    }
}
