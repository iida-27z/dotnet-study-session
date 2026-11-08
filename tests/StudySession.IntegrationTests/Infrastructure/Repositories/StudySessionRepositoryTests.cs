using System.ComponentModel;
using StudySession.Domain.StudySessions;
using StudySession.Infrastructure.Repositories;
using StudySession.IntegrationTests.Infrastructure.Database;

namespace StudySession.IntegrationTests.Infrastructure.Repositories;

[Trait("Category", "Integration")]
public class StudySessionRepositoryTests : IClassFixture<PostgresTestContainerFixture>
{
    private readonly StudySessionRepository _repository;

    public StudySessionRepositoryTests(PostgresTestContainerFixture fixture)
    {
        _repository = new StudySessionRepository(fixture.CreateConnectionFactory());
    }

    [Fact]
    [DisplayName("FindByStudySessionIdAsyncメソッドで存在するセッションIDを指定するとセッションを返す")]
    public async Task FindByStudySessionIdAsync_WithExistingSessionId_ReturnsStudySession()
    {
        // arrange
        var sessionId = StudySessionId.From(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"));

        // act
        var session = await _repository.FindByStudySessionIdAsync(sessionId);

        // assert
        Assert.NotNull(session);
        Assert.Equal("SQL入門セミナー", session!.Title);
        Assert.Equal(StudySessionFormat.Hybrid, session.Format);
        Assert.Equal(10, session.CapacityOnsite);
        Assert.Equal(20, session.CapacityRemote);
    }

    [Fact]
    [DisplayName("FindByStartTimeOnOrAfterOrderByStartTimeAsyncメソッドで指定時刻以降のセッションを開始日時順で返す")]
    public async Task FindByStartTimeOnOrAfterOrderByStartTimeAsync_WithStartTime_ReturnsOrderedSessions()
    {
        // arrange
        var now = DateTime.UtcNow;

        // act
        var sessions = await _repository.FindByStartTimeOnOrAfterOrderByStartTimeAsync(now);

        // assert
        Assert.NotEmpty(sessions);
        Assert.True(sessions.Zip(sessions.Skip(1), (current, next) => current.StartTime <= next.StartTime).All(x => x));
    }
}
