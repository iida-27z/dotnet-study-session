using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudySession.Application.StudySessions;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;
using StudySession.Web.Pages.StudySessions;

namespace StudySession.WebTests;

public class StudySessionCreatePageTests
{
    [Fact]
    [DisplayName("正常な入力で登録すると一覧画面へリダイレクトする")]
    public async Task OnPostAsync_WithValidRequest_RedirectsToStudySessionList()
    {
        // arrange
        var repository = new FakeStudySessionRepository();
        var pageModel = new CreateModel(new CreateStudySessionUseCase(repository))
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
        var pageModel = new CreateModel(new CreateStudySessionUseCase(repository))
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

    private sealed class FakeStudySessionRepository : IStudySessionRepository
    {
        public List<StudySession> SavedSessions { get; } = [];

        public Task<StudySession?> FindByStudySessionIdAsync(StudySessionId studySessionId, CancellationToken cancellationToken = default)
            => Task.FromResult<StudySession?>(null);

        public Task<List<StudySession>> FindByStartTimeOnOrAfterOrderByStartTimeAsync(DateTime startTime, CancellationToken cancellationToken = default)
            => Task.FromResult(new List<StudySession>());

        public Task AddAsync(StudySession studySession, CancellationToken cancellationToken = default)
        {
            SavedSessions.Add(studySession);
            return Task.CompletedTask;
        }

        public Task SaveAsync(StudySession studySession, CancellationToken cancellationToken = default)
            => AddAsync(studySession, cancellationToken);
    }
}
