using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudySession.Application.StudySessions;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;
using StudySession.Web.Pages.StudySessions;
using StudySessionEntity = StudySession.Domain.StudySessions.StudySession;

namespace StudySession.WebTests;

public class StudySessionCreatePageTests
{
    [Fact]
    [DisplayName("正常な入力で登録すると一覧画面へリダイレクトする")]
    public async Task OnPostAsync_WithValidRequest_RedirectsToStudySessionList()
    {
        // arrange
        var repository = new FakeStudySessionRepository();
        var pageModel = new CreateModel(new CreateStudySessionUseCase(repository, new StudySessionTimeZone()), new StudySessionTimeZone())
        {
            Title = "Spring Boot 3.x 移行実践ワークショップ",
            StartTime = new DateTime(2026, 12, 15, 10, 0, 0),
            EndTime = new DateTime(2026, 12, 15, 12, 0, 0),
            Format = StudySessionFormat.Hybrid.ToString(),
            CapacityOnsite = 10,
            CapacityRemote = 20,
            Description = "Spring Boot 3.x への移行を実務視点で学ぶ会です。"
        };
        pageModel.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, UserId.New().ToString())],
                    "TestAuth"))
            }
        };

        // act
        var result = await pageModel.OnPostAsync(CancellationToken.None);

        // assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/StudySessions/List", redirect.PageName);
        Assert.Single(repository.SavedSessions);
        Assert.Equal("Spring Boot 3.x 移行実践ワークショップ", repository.SavedSessions[0].Title);
    }

    [Fact]
    [DisplayName("Hybridでは対面枠またはリモート枠が0だと登録できない")]
    public async Task OnPostAsync_WithHybridCapacityConstraintInvalid_ReturnsPageWithError()
    {
        // arrange
        var repository = new FakeStudySessionRepository();
        var pageModel = new CreateModel(new CreateStudySessionUseCase(repository, new StudySessionTimeZone()), new StudySessionTimeZone())
        {
            Title = "ハイブリッド勉強会",
            StartTime = new DateTime(2026, 12, 15, 10, 0, 0),
            EndTime = new DateTime(2026, 12, 15, 12, 0, 0),
            Format = StudySessionFormat.Hybrid.ToString(),
            CapacityOnsite = 0,
            CapacityRemote = 20,
            Description = "概要"
        };
        pageModel.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, UserId.New().ToString())],
                    "TestAuth"))
            }
        };

        // act
        var result = await pageModel.OnPostAsync(CancellationToken.None);

        // assert
        Assert.IsType<PageResult>(result);
        Assert.False(pageModel.ModelState.IsValid);
        Assert.True(pageModel.ModelState.ContainsKey(nameof(CreateModel.CapacityOnsite)));
        Assert.Empty(repository.SavedSessions);
    }

    [Fact]
    [DisplayName("ブラウザのJST入力はUTCに変換して保存する")]
    public async Task OnPostAsync_WithJstInput_StoresUtcTime()
    {
        // arrange
        var repository = new FakeStudySessionRepository();
        var pageModel = new CreateModel(new CreateStudySessionUseCase(repository, new StudySessionTimeZone()), new StudySessionTimeZone())
        {
            Title = "JST入力テスト",
            StartTime = new DateTime(2026, 12, 15, 10, 0, 0, DateTimeKind.Unspecified),
            EndTime = new DateTime(2026, 12, 15, 12, 0, 0, DateTimeKind.Unspecified),
            Format = StudySessionFormat.Hybrid.ToString(),
            CapacityOnsite = 10,
            CapacityRemote = 20,
            Description = "JST入力のテストです。"
        };
        pageModel.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, UserId.New().ToString())],
                    "TestAuth"))
            }
        };

        // act
        var result = await pageModel.OnPostAsync(CancellationToken.None);

        // assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/StudySessions/List", redirect.PageName);
        var saved = Assert.Single(repository.SavedSessions);
        Assert.Equal(new DateTime(2026, 12, 15, 1, 0, 0, DateTimeKind.Utc), saved.StartTime);
        Assert.Equal(new DateTime(2026, 12, 15, 3, 0, 0, DateTimeKind.Utc), saved.EndTime);
    }

    [Fact]
    [DisplayName("ブラウザISOのタイムゾーン付き入力はUTCに変換して保存する")]
    public async Task OnPostAsync_WithOffsetIsoInput_StoresUtcTime()
    {
        // arrange
        var repository = new FakeStudySessionRepository();
        var pageModel = new CreateModel(new CreateStudySessionUseCase(repository, new StudySessionTimeZone()), new StudySessionTimeZone())
        {
            Title = "TZ付き入力テスト",
            StartTime = new DateTime(2026, 12, 15, 10, 0, 0, DateTimeKind.Unspecified),
            EndTime = new DateTime(2026, 12, 15, 12, 0, 0, DateTimeKind.Unspecified),
            StartTimeUtcValue = "2026-12-15T01:00:00.000Z",
            EndTimeUtcValue = "2026-12-15T03:00:00.000Z",
            Format = StudySessionFormat.Hybrid.ToString(),
            CapacityOnsite = 10,
            CapacityRemote = 20,
            Description = "TZ付き入力のテストです。"
        };
        pageModel.PageContext = new PageContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, UserId.New().ToString())],
                    "TestAuth"))
            }
        };

        // act
        var result = await pageModel.OnPostAsync(CancellationToken.None);

        // assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/StudySessions/List", redirect.PageName);
        var saved = Assert.Single(repository.SavedSessions);
        Assert.Equal(new DateTime(2026, 12, 15, 1, 0, 0, DateTimeKind.Utc), saved.StartTime);
        Assert.Equal(new DateTime(2026, 12, 15, 3, 0, 0, DateTimeKind.Utc), saved.EndTime);
    }

    [Fact]
    [DisplayName("注入した TimeProvider を使って現在時刻を取得する")]
    public void UtcNow_UsesInjectedTimeProvider()
    {
        var expected = new DateTimeOffset(2026, 12, 15, 12, 0, 0, TimeSpan.Zero);
        var clock = new StudySessionTimeZone(new FixedTimeProvider(expected));

        Assert.Equal(expected.UtcDateTime, clock.UtcNow);
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private sealed class FakeStudySessionRepository : IStudySessionRepository
    {
        public List<StudySessionEntity> SavedSessions { get; } = [];

        public Task<StudySessionEntity?> FindByStudySessionIdAsync(StudySessionId studySessionId, CancellationToken cancellationToken = default)
            => Task.FromResult<StudySessionEntity?>(null);

        public Task<List<StudySessionEntity>> FindByStartTimeOnOrAfterOrderByStartTimeAsync(DateTime startTime, CancellationToken cancellationToken = default)
            => Task.FromResult(new List<StudySessionEntity>());

        public Task AddAsync(StudySessionEntity studySession, CancellationToken cancellationToken = default)
        {
            SavedSessions.Add(studySession);
            return Task.CompletedTask;
        }

        public Task SaveAsync(StudySessionEntity studySession, CancellationToken cancellationToken = default)
            => AddAsync(studySession, cancellationToken);
    }
}
