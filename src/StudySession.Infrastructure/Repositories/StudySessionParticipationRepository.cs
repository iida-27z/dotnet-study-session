using Dapper;
using StudySession.Domain.Participation;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;
using StudySession.Infrastructure.Data;

namespace StudySession.Infrastructure.Repositories;

/// <summary>
/// Dapper を利用して勉強会参加情報を永続化するリポジトリ実装です。
/// </summary>
public sealed class StudySessionParticipationRepository : IStudySessionParticipationRepository
{
    private readonly IStudySessionDbConnectionFactory _connectionFactory;

    /// <summary>
    /// <see cref="StudySessionParticipationRepository"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="connectionFactory">データベース接続ファクトリ。</param>
    public StudySessionParticipationRepository(IStudySessionDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc/>
    public async Task SaveAsync(StudySessionParticipation participation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(participation);

        const string sql = """
            INSERT INTO session_participations (
                session_id,
                user_id,
                attendance_status,
                comment,
                responded_at,
                updated_at
            ) VALUES (
                @SessionId,
                @UserId,
                @AttendanceStatus,
                @Comment,
                @RespondedAt,
                @UpdatedAt
            )
            ON CONFLICT (session_id, user_id) DO UPDATE SET
                attendance_status = EXCLUDED.attendance_status,
                comment = EXCLUDED.comment,
                responded_at = EXCLUDED.responded_at,
                updated_at = EXCLUDED.updated_at
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                SessionId = participation.SessionId.Value,
                UserId = participation.UserId.Value,
                AttendanceStatus = participation.AttendanceStatus.ToString(),
                Comment = participation.Comment,
                RespondedAt = participation.RespondedAt,
                UpdatedAt = participation.UpdatedAt,
            }, cancellationToken: cancellationToken));
    }

    /// <inheritdoc/>
    public async Task<StudySessionParticipation?> FindBySessionAndUserIdAsync(StudySessionId sessionId, UserId userId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                sp.session_id AS SessionId,
                sp.user_id AS UserId,
                sp.attendance_status AS AttendanceStatus,
                sp.comment AS Comment,
                sp.responded_at AS RespondedAt,
                sp.updated_at AS UpdatedAt,
                u.login_id AS LoginId,
                u.display_name AS DisplayName
            FROM session_participations sp
            LEFT JOIN users u ON u.user_id = sp.user_id
            WHERE sp.session_id = @SessionId AND sp.user_id = @UserId
            LIMIT 1
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<ParticipationRow>(
            new CommandDefinition(sql, new { SessionId = sessionId.Value, UserId = userId.Value }, cancellationToken: cancellationToken));

        return row is null ? null : MapToDomainEntity(row);
    }

    /// <inheritdoc/>
    public async Task<List<StudySessionParticipation>> FindBySessionIdAsync(StudySessionId sessionId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                sp.session_id AS SessionId,
                sp.user_id AS UserId,
                sp.attendance_status AS AttendanceStatus,
                sp.comment AS Comment,
                sp.responded_at AS RespondedAt,
                sp.updated_at AS UpdatedAt,
                u.login_id AS LoginId,
                u.display_name AS DisplayName
            FROM session_participations sp
            LEFT JOIN users u ON u.user_id = sp.user_id
            WHERE sp.session_id = @SessionId
            ORDER BY u.display_name NULLS LAST, sp.user_id
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<ParticipationRow>(
            new CommandDefinition(sql, new { SessionId = sessionId.Value }, cancellationToken: cancellationToken));

        return rows.Select(MapToDomainEntity).ToList();
    }

    private static StudySessionParticipation MapToDomainEntity(ParticipationRow row)
    {
        var attendanceStatus = Enum.TryParse<AttendanceStatus>(row.AttendanceStatus, out var status)
            ? status
            : AttendanceStatus.Unanswered;

        return new StudySessionParticipation(
            StudySessionId.From(row.SessionId),
            UserId.From(row.UserId),
            attendanceStatus,
            row.Comment,
            row.RespondedAt,
            row.UpdatedAt,
            row.DisplayName,
            row.LoginId);
    }

    private sealed record ParticipationRow(
        Guid SessionId,
        Guid UserId,
        string AttendanceStatus,
        string? Comment,
        DateTime? RespondedAt,
        DateTime UpdatedAt,
        string? LoginId,
        string? DisplayName);
}
