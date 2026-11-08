using System.ComponentModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using StudySession.Application.StudySessions;
using StudySession.Domain.Participation;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;
using StudySession.Web.Pages.StudySessions;
using StudySession.Web.Time;
using StudySessionEntity = StudySession.Domain.StudySessions.StudySession;

namespace StudySession.WebTests;

public class StudySessionDetailPageTests
{
    [Fact]
    [DisplayName("有効なセッションIDで取得すると詳細情報と参加者一覧が読み込まれる")]
    public async Task OnGetAsync_WithValidSessionId_LoadsSessionDetails()
    {
        // arrange
        var sessionId = StudySessionId.New();
        var now = DateTimeOffset.UtcNow;
        var participantRepo = new FakeStudySessionParticipationRepository(
            new StudySessionParticipation(sessionId, UserId.New(), AttendanceStatus.Onsite, "実機で参加します。", now, now),
            new StudySessionParticipation(sessionId, UserId.New(), AttendanceStatus.Absent, "欠席予定です。", now, now));
        var repository = new FakeStudySessionRepository(CreateSession(sessionId, StudySessionFormat.OnsiteOnly));
        var timeProvider = new FakeDateTimeProvider(now);
        var pageModel = new DetailModel(repository, participantRepo, CreateDateTimeConverter(), timeProvider)
        {
            Id = sessionId.ToString(),
            PageContext = CreatePageContext(UserId.New()),
        };

        // act
        var result = await pageModel.OnGetAsync(CancellationToken.None);

        // assert
        var page = Assert.IsType<PageResult>(result);
        Assert.NotNull(pageModel.Session);
        Assert.Equal("ネットワーク実習", pageModel.Session!.Title);
        Assert.Equal(StudySessionFormat.OnsiteOnly, pageModel.Session.Format);
        Assert.Contains(pageModel.Participants, p => p.Status == AttendanceStatus.Onsite || p.Status == AttendanceStatus.Absent);
    }

    [Fact]
    [DisplayName("OnsiteOnlyではRemote選択時にエラーを返す")]
    public async Task OnPostAsync_WithRemoteSelectionForOnsiteOnly_ReturnsPageWithError()
    {
        // arrange
        var session = CreateSession(StudySessionId.New(), StudySessionFormat.OnsiteOnly);
        var repository = new FakeStudySessionRepository(session);
        var participantRepo = new FakeStudySessionParticipationRepository();
        var timeProvider = new FakeDateTimeProvider(DateTimeOffset.UtcNow);
        var pageModel = new DetailModel(repository, participantRepo, CreateDateTimeConverter(), timeProvider)
        {
            Id = session.SessionId.ToString(),
            AttendanceStatusValue = AttendanceStatus.Remote.ToString(),
            Comment = "リモート参加希望です。",
            PageContext = CreatePageContext(UserId.New()),
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
        var participantRepo = new FakeStudySessionParticipationRepository();
        var timeProvider = new FakeDateTimeProvider(DateTimeOffset.UtcNow);
        var pageModel = new DetailModel(repository, participantRepo, CreateDateTimeConverter(), timeProvider)
        {
            Id = session.SessionId.ToString(),
            AttendanceStatusValue = AttendanceStatus.Onsite.ToString(),
            Comment = "事前に資料を確認します。",
            PageContext = CreatePageContext(UserId.New()),
        };

        // act
        var result = await pageModel.OnPostAsync(CancellationToken.None);

        // assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/StudySessions/List", redirect.PageName);
    }

    [Fact]
    [DisplayName("参加者名簿のユーザーID表示は login_id を使う")]
    public async Task OnGetAsync_WithLoginIdLoaded_UsesLoginIdForParticipantList()
    {
        // arrange
        var sessionId = StudySessionId.New();
        var userId = UserId.New();
        var now = DateTimeOffset.UtcNow;
        var repository = new FakeStudySessionRepository(CreateSession(sessionId, StudySessionFormat.OnsiteOnly));
        var participantRepo = new FakeStudySessionParticipationRepository(
            new StudySessionParticipation(sessionId, userId, AttendanceStatus.Onsite, "実機で参加します。", now, now, "開発 太郎", "dev"));
        var timeProvider = new FakeDateTimeProvider(now);
        var pageModel = new DetailModel(repository, participantRepo, CreateDateTimeConverter(), timeProvider)
        {
            Id = sessionId.ToString(),
            PageContext = CreatePageContext(UserId.New()),
        };

        // act
        var result = await pageModel.OnGetAsync(CancellationToken.None);

        // assert
        var page = Assert.IsType<PageResult>(result);
        var participant = Assert.Single(pageModel.Participants);
        Assert.Equal("dev", participant.LoginId);
    }

    private static IDateTimeConverter CreateDateTimeConverter()
        => new PresentationDateTimeConverter(new OptionsWrapper<TimeZoneSettings>(new TimeZoneSettings { DefaultTimeZoneId = "Asia/Tokyo" }));

    private static PageContext CreatePageContext(UserId userId)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())],
            "TestAuth"));

        return new PageContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal,
            },
        };
    }

    private static StudySessionEntity CreateSession(StudySessionId sessionId, StudySessionFormat format = StudySessionFormat.Hybrid)
    {
        var now = DateTimeOffset.UtcNow;
        return new StudySessionEntity(
            sessionId,
            "ネットワーク実習",
            "社内ネットワーク構築の基礎を学ぶハンズオンセミナーです。",
            new DateTimeOffset(2026, 12, 12, 17, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 12, 12, 19, 0, 0, TimeSpan.Zero),
            format,
            format == StudySessionFormat.RemoteOnly ? 0 : 20,
            format == StudySessionFormat.OnsiteOnly ? 0 : 30,
            UserId.New(),
            now,
            now);
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

        public Task<List<StudySessionEntity>> FindByStartTimeOnOrAfterOrderByStartTimeAsync(DateTimeOffset startTime, CancellationToken cancellationToken = default)
            => Task.FromResult(new List<StudySessionEntity> { _session });

        public Task SaveAsync(StudySessionEntity studySession, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakeDateTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public FakeDateTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }

    private sealed class FakeStudySessionParticipationRepository : IStudySessionParticipationRepository
    {
        private readonly List<StudySessionParticipation> _participations;

        public FakeStudySessionParticipationRepository(params StudySessionParticipation[] participations)
        {
            _participations = [.. participations];
        }

        public Task<StudySessionParticipation?> FindBySessionAndUserIdAsync(StudySessionId sessionId, UserId userId, CancellationToken cancellationToken = default)
            => Task.FromResult(_participations.FirstOrDefault(x => x.SessionId == sessionId && x.UserId == userId));

        public Task<List<StudySessionParticipation>> FindBySessionIdAsync(StudySessionId sessionId, CancellationToken cancellationToken = default)
            => Task.FromResult(_participations.Where(x => x.SessionId == sessionId).ToList());

        public Task SaveAsync(StudySessionParticipation participation, CancellationToken cancellationToken = default)
        {
            var existingIndex = _participations.FindIndex(x => x.SessionId == participation.SessionId && x.UserId == participation.UserId);
            if (existingIndex >= 0)
            {
                _participations[existingIndex] = participation;
                return Task.CompletedTask;
            }

            _participations.Add(participation);
            return Task.CompletedTask;
        }
    }
}
