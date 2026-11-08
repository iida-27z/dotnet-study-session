using System.ComponentModel;
using StudySession.Domain.Participation;
using StudySession.Domain.Users;
using StudySession.Infrastructure.QueryServices;
using StudySession.IntegrationTests.Infrastructure.Database;

namespace StudySession.IntegrationTests.Infrastructure.QueryServices;

[Trait("Category", "Integration")]
public class StudySessionParticipationSummaryQueryServiceTests : IClassFixture<PostgresTestContainerFixture>
{
    private readonly StudySessionParticipationSummaryQueryService _queryService;

    public StudySessionParticipationSummaryQueryServiceTests(PostgresTestContainerFixture fixture)
    {
        _queryService = new StudySessionParticipationSummaryQueryService(fixture.CreateConnectionFactory());
    }

    [Fact]
    [DisplayName("GetParticipationSummaryAsyncメソッドで指定ユーザーの参加状況サマリーが取得できる")]
    public async Task GetParticipationSummaryAsync_WithUserId_ReturnsSessionSummaries()
    {
        // arrange
        var userId = UserId.From(Guid.Parse("22222222-2222-4222-8222-222222222222"));

        // act
        var summaries = await _queryService.GetParticipationSummaryAsync(DateTime.Today, userId, CancellationToken.None);

        // assert
        Assert.NotEmpty(summaries);
        Assert.Collection(summaries.OrderBy(x => x.StartTime),
            summary =>
            {
                Assert.Equal("SQL入門セミナー", summary.Title);
                Assert.Equal(1, summary.ParticipantsCountOnsite);
                Assert.Equal(1, summary.ParticipantsCountRemote);
                Assert.Equal(UserId.From(Guid.Parse("22222222-2222-4222-8222-222222222222")), summary.UserId);
                Assert.Equal(AttendanceStatus.Unanswered, summary.IsAttendingOnsite);
                Assert.Equal(AttendanceStatus.Remote, summary.IsAttendingRemote);
            },
            summary =>
            {
                Assert.Equal("テスト設計の基礎", summary.Title);
                Assert.Equal(1, summary.ParticipantsCountOnsite);
                Assert.Equal(0, summary.ParticipantsCountRemote);
                Assert.Equal(UserId.From(Guid.Parse("22222222-2222-4222-8222-222222222222")), summary.UserId);
                Assert.Equal(AttendanceStatus.Unanswered, summary.IsAttendingOnsite);
                Assert.Equal(AttendanceStatus.Unanswered, summary.IsAttendingRemote);
            },
            summary =>
            {
                Assert.Equal("Dockerと開発環境整備", summary.Title);
                Assert.Equal(0, summary.ParticipantsCountOnsite);
                Assert.Equal(1, summary.ParticipantsCountRemote);
                Assert.Equal(UserId.From(Guid.Parse("22222222-2222-4222-8222-222222222222")), summary.UserId);
                Assert.Equal(AttendanceStatus.Unanswered, summary.IsAttendingOnsite);
                Assert.Equal(AttendanceStatus.Remote, summary.IsAttendingRemote);
            });
    }
}
