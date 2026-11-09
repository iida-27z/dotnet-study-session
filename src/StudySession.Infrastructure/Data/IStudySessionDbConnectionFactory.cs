using System.Data.Common;

namespace StudySession.Infrastructure.Data;

/// <summary>
/// Creates open database connections for the application.
/// </summary>
public interface IStudySessionDbConnectionFactory
{
    /// <summary>
    /// Creates and opens a database connection.
    /// </summary>
    ValueTask<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
