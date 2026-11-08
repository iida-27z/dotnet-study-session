using Dapper;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;
using StudySession.Infrastructure.Data;
using StudySessionEntity = StudySession.Domain.StudySessions.StudySession;

namespace StudySession.Infrastructure.Repositories;

/// <summary>
/// Dapper を利用して勉強会データを永続化するリポジトリ実装です。
/// </summary>
public sealed class StudySessionRepository : IStudySessionRepository
{
    private readonly IStudySessionDbConnectionFactory _connectionFactory;

    /// <summary>
    /// <see cref="StudySessionRepository"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="connectionFactory">データベース接続ファクトリ。</param>
    public StudySessionRepository(IStudySessionDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc/>
    public async Task<List<StudySessionEntity>> FindByStartTimeOnOrAfterOrderByStartTimeAsync(DateTime startTime, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                session_id AS StudySessionId,
                title AS Title,
                description AS Description,
                starts_at AS StartsAt,
                ends_at AS EndsAt,
                format AS Format,
                capacity_onsite AS CapacityOnsite,
                capacity_remote AS CapacityRemote,
                created_by_user_id AS CreatedByUserId,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM study_sessions
            WHERE starts_at >= @StartTime
            ORDER BY starts_at
        """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<StudySessionRow>(
            new CommandDefinition(sql, new { StartTime = startTime }, cancellationToken: cancellationToken));

        return [.. rows.Select(MapToDomainEntity)];
    }

    /// <inheritdoc/>
    public async Task<StudySessionEntity?> FindByStudySessionIdAsync(StudySessionId studySessionId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                session_id AS StudySessionId,
                title AS Title,
                description AS Description,
                starts_at AS StartsAt,
                ends_at AS EndsAt,
                format AS Format,
                capacity_onsite AS CapacityOnsite,
                capacity_remote AS CapacityRemote,
                created_by_user_id AS CreatedByUserId,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM study_sessions
            WHERE session_id = @StudySessionId
            LIMIT 1
        """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<StudySessionRow>(
            new CommandDefinition(sql, new { StudySessionId = studySessionId.Value }, cancellationToken: cancellationToken));
        if (row is null)
        {
            return null;
        }

        return MapToDomainEntity(row);
    }

    private static StudySessionEntity MapToDomainEntity(StudySessionRow row)
    {
        if (!Enum.TryParse(row.Format, out StudySessionFormat format))
        {
            throw new InvalidOperationException($"Unsupported format value '{row.Format}'.");
        }

        return new StudySessionEntity(
            StudySessionId.From(row.StudySessionId),
            row.Title,
            row.Description ?? string.Empty,
            row.StartsAt,
            row.EndsAt,
            format,
            row.CapacityOnsite,
            row.CapacityRemote,
            UserId.From(row.CreatedByUserId),
            row.CreatedAt,
            row.UpdatedAt);
    }

    private sealed record StudySessionRow(
        Guid StudySessionId,
        string Title,
        string? Description,
        DateTime StartsAt,
        DateTime EndsAt,
        string Format,
        int CapacityOnsite,
        int CapacityRemote,
        Guid CreatedByUserId,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
