using System.ComponentModel;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;
using StudySessionEntity = StudySession.Domain.StudySessions.StudySession;

namespace StudySession.DomainTests;

public class StudySessionTests
{
    [Fact]
    [DisplayName("コンストラクタで開始日時が終了日時以上の場合は例外が発生する")]
    public void Constructor_WithStartTimeNotBeforeEndTime_ThrowsArgumentException()
    {
        // arrange
        var startTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var endTime = startTime;

        // act & assert
        Assert.Throws<ArgumentException>(() => new StudySessionEntity(
            StudySessionId.New(),
            "タイトル",
            "説明",
            startTime,
            endTime,
            StudySessionFormat.Hybrid,
            10,
            20,
            UserId.New(),
            startTime,
            endTime));
    }

    [Fact]
    [DisplayName("コンストラクタで空のセッションIDを渡すと例外が発生する")]
    public void Constructor_WithEmptySessionId_ThrowsArgumentException()
    {
        // arrange
        var startTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var endTime = startTime.AddHours(1);

        // act & assert
        Assert.Throws<ArgumentException>(() => new StudySessionEntity(
            StudySessionId.From(Guid.Empty),
            "タイトル",
            "説明",
            startTime,
            endTime,
            StudySessionFormat.RemoteOnly,
            10,
            20,
            UserId.New(),
            startTime,
            endTime));
    }
}
