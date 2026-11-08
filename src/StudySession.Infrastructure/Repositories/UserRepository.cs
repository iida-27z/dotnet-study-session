using Dapper;
using StudySession.Domain.Users;
using StudySession.Infrastructure.Data;

namespace StudySession.Infrastructure.Repositories;

/// <summary>
/// Dapper-based user repository implementation.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly IStudySessionDbConnectionFactory _connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    public UserRepository(IStudySessionDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc/>
    public async Task<User?> FindByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                user_id AS UserId,
                login_id AS LoginId,
                display_name AS DisplayName,
                email AS Email,
                password_hash AS PasswordHash,
                role AS Role,
                is_active AS IsActive
            FROM users
            WHERE user_id = @UserId
            LIMIT 1
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<UserRow>(sql, new { UserId = userId.Value });

        if (row is null)
        {
            return null;
        }

        if (!Enum.TryParse<UserRole>(row.Role, out UserRole role))
        {
            throw new InvalidOperationException($"Unsupported role value '{row.Role}'.");
        }

        return new User(
            UserId.From(row.UserId),
            row.LoginId,
            row.DisplayName,
            row.Email,
            row.PasswordHash,
            role,
            row.IsActive);
    }

    /// <inheritdoc/>
    public async Task<User?> FindByLoginIdAsync(string loginId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                user_id AS UserId,
                login_id AS LoginId,
                display_name AS DisplayName,
                email AS Email,
                password_hash AS PasswordHash,
                role AS Role,
                is_active AS IsActive
            FROM users
            WHERE login_id = @LoginId
            LIMIT 1
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<UserRow>(sql, new { LoginId = loginId });

        if (row is null)
        {
            return null;
        }

        if (!Enum.TryParse<UserRole>(row.Role, out UserRole role))
        {
            throw new InvalidOperationException($"Unsupported role value '{row.Role}'.");
        }

        return new User(
            UserId.From(row.UserId),
            row.LoginId,
            row.DisplayName,
            row.Email,
            row.PasswordHash,
            role,
            row.IsActive);
    }

    private sealed record UserRow(
        Guid UserId,
        string LoginId,
        string DisplayName,
        string Email,
        string PasswordHash,
        string Role,
        bool IsActive);
}
