using System.Data.Common;
using Npgsql;
using StudySession.Infrastructure.Data;
using Testcontainers.PostgreSql;

namespace StudySession.IntegrationTests.Infrastructure.Database;

/// <summary>
/// 統合テスト用の PostgreSQL コンテナのライフサイクル管理および接続情報を提供するフィクスチャクラス。
/// </summary>
public sealed class PostgresTestContainerFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    private const string image = "postgres:18.6";
    private const string nonRootDbUsername = "app_user";
    private const string nonRootDbPassword = "app_pass";
    private const string dbSchema = "app";
    private readonly string initScriptsDirectory = Path.Combine(AppContext.BaseDirectory, "init"); // csprojファイルにbin 配下にファイルをコピーする設定を行っているため、ここで参照可能

    /// <summary>
    /// PostgreSQL コンテナへの接続文字列を取得します。
    /// この接続文字列は、非ルートユーザーの認証情報を使用して作成されます。
    /// </summary>
    public string ConnectionString => CreateConnectionString(nonRootDbUsername, nonRootDbPassword);

    /// <summary>
    /// PostgreSQL コンテナ用の接続ファクトリを作成します。
    /// </summary>
    public IStudySessionDbConnectionFactory CreateConnectionFactory()
    {
        return new TestStudySessionDbConnectionFactory(ConnectionString);
    }

    public async Task InitializeAsync()
    {
        var builder = new PostgreSqlBuilder(image)
            .WithEnvironment("APP_USER", nonRootDbUsername)
            .WithEnvironment("APP_PASSWORD", nonRootDbPassword)
            .WithEnvironment("APP_SCHEMA", dbSchema)
            .WithResourceMapping(new DirectoryInfo(initScriptsDirectory), "/docker-entrypoint-initdb.d");

        _container = builder.Build();
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }

    private string CreateConnectionString(string username, string password)
    {
        if (_container is null)
        {
            throw new InvalidOperationException("The PostgreSQL test container has not been started yet.");
        }

        var builder = new NpgsqlConnectionStringBuilder(_container.GetConnectionString())
        {
            Username = username,
            Password = password,
            SearchPath = dbSchema
        };

        return builder.ConnectionString;
    }

    private sealed class TestStudySessionDbConnectionFactory : IStudySessionDbConnectionFactory
    {
        private readonly string _connectionString;

        public TestStudySessionDbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async ValueTask<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }
}
