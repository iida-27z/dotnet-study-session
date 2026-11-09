using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace StudySession.Infrastructure.Data;

/// <summary>
/// PostgreSQL connection factory using environment-backed configuration.
/// </summary>
public sealed class StudySessionDbConnectionFactory : IStudySessionDbConnectionFactory
{
    private const string StudyDbConnectionKey = "STUDY_DB_CONNECTION";
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="StudySessionDbConnectionFactory"/> class.
    /// </summary>
    public StudySessionDbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public async ValueTask<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connectionString = _configuration[StudyDbConnectionKey]
            ?? _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Database connection string not found. Set '{StudyDbConnectionKey}' or 'ConnectionStrings:DefaultConnection'.");
        }

        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
