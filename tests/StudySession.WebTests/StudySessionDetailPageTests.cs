using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudySession.Domain.Participation;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;
using StudySession.Web.Pages.StudySessions;
using StudySessionEntity = StudySession.Domain.StudySessions.StudySession;

namespace StudySession.WebTests;

public class StudySessionDetailPageTests
{
    [Fact]
    [DisplayName("有効なセッションIDで取得すると詳細情報が読み込まれる")]
    public async Task OnGetAsync_WithValidSessionId_LoadsSessionDetails()
    {
        // arrange
        var sessionId = StudySessionId.New();
        var repository = new FakeStudySessionRepository(CreateSession(sessionId, StudySessionFormat.OnsiteOnly));
        var pageModel = new DetailModel(repository)
        {
            Id = sessionId.ToString(),
        };

        // act
        var result = await pageModel.OnGetAsync(CancellationToken.None);

        // assert
        var page = Assert.IsType<PageResult>(result);
        Assert.NotNull(pageModel.Session);
        Assert.Equal("ネットワーク実習", pageModel.Session!.Title);
        Assert.Equal(StudySessionFormat.OnsiteOnly, pageModel.Session.Format);
    }

    [Fact]
    [DisplayName("OnsiteOnlyではRemote選択時にエラーを返す")]
    public async Task OnPostAsync_WithRemoteSelectionForOnsiteOnly_ReturnsPageWithError()
    {
        // arrange
        var session = CreateSession(StudySessionId.New(), StudySessionFormat.OnsiteOnly);
        var repository = new FakeStudySessionRepository(session);
        var pageModel = new DetailModel(repository)
        {
            Id = session.SessionId.ToString(),
            AttendanceStatusValue = AttendanceStatus.Remote.ToString(),
            Comment = "リモート参加希望です。",
        };

        // act
        var result = await pageModel.OnPostAsync(CancellationToken.None);

        // assert
        Assert.IsType<PageResult>(result);
        Assert.False(pageModel.ModelState.IsValid);
        Assert.True(pageModel.ModelState.ContainsKey(nameof(DetailModel.AttendanceStatusValue)));
    }

    [Fact]
    [DisplayName("有効な出欠更新で一覧画面へリダイレクトする")]
    public async Task OnPostAsync_WithValidAttendanceRequest_RedirectsToListPage()
    {
        // arrange
        var session = CreateSession(StudySessionId.New(), StudySessionFormat.Hybrid);
        var repository = new FakeStudySessionRepository(session);
        var pageModel = new DetailModel(repository)
        {
            Id = session.SessionId.ToString(),
            AttendanceStatusValue = AttendanceStatus.Onsite.ToString(),
            Comment = "事前に資料を確認します。",
        };

        // act
        var result = await pageModel.OnPostAsync(CancellationToken.None);

        // assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/StudySessions/List", redirect.PageName);
    }

    private static StudySessionEntity CreateSession(StudySessionId sessionId, StudySessionFormat format = StudySessionFormat.Hybrid)
    {
        return new StudySessionEntity(
            sessionId,
            "ネットワーク実習",
            "社内ネットワーク構築の基礎を学ぶハンズオンセミナーです。",
            new DateTime(2026, 12, 12, 17, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 12, 12, 19, 0, 0, DateTimeKind.Utc),
            format,
            format == StudySessionFormat.RemoteOnly ? 0 : 20,
            format == StudySessionFormat.OnsiteOnly ? 0 : 30,
            UserId.New(),
            DateTime.UtcNow,
            DateTime.UtcNow);
    }

    private sealed class FakeStudySessionRepository : IStudySessionRepository
    {
        private readonly StudySessionEntity _session;

        public FakeStudySessionRepository(StudySessionEntity session)
        {
            _session = session;
        }

        public Task AddAsync(StudySessionEntity studySession, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<StudySessionEntity?> FindByStudySessionIdAsync(StudySessionId studySessionId, CancellationToken cancellationToken = default)
            => Task.FromResult(studySessionId == _session.SessionId ? _session : null);

        public Task<List<StudySessionEntity>> FindByStartTimeOnOrAfterOrderByStartTimeAsync(DateTime startTime, CancellationToken cancellationToken = default)
            => Task.FromResult(new List<StudySessionEntity> { _session });

        public Task SaveAsync(StudySessionEntity studySession, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
