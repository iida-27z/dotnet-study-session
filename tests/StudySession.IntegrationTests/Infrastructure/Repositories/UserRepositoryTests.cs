using System.ComponentModel;
using StudySession.Domain.Users;
using StudySession.Infrastructure.Repositories;
using StudySession.IntegrationTests.Infrastructure.Database;

namespace StudySession.IntegrationTests.Infrastructure.Repositories;

[Trait("Category", "Integration")]
public class UserRepositoryTests : IClassFixture<PostgresTestContainerFixture>
{
    private readonly UserRepository _repository;

    public UserRepositoryTests(PostgresTestContainerFixture fixture)
    {
        _repository = new UserRepository(fixture.CreateConnectionFactory());
    }

    [Fact]
    [DisplayName("FindByUserIdAsyncメソッドで存在するユーザーIDを指定するとユーザーを返す")]
    public async Task FindByUserIdAsync_WithExistingUserId_ReturnsUser()
    {
        // arrange
        var userId = UserId.From(Guid.Parse("11111111-1111-4111-8111-111111111111"));

        // act
        var user = await _repository.FindByUserIdAsync(userId);

        // assert
        Assert.NotNull(user);
        Assert.Equal("admin", user!.LoginId);
        Assert.Equal("管理者 太郎", user.DisplayName);
        Assert.Equal("admin@example.com", user.Email);
        Assert.Equal(UserRole.Admin, user.Role);
        Assert.True(user.IsActive);
    }

    [Fact]
    [DisplayName("FindByLoginIdAsyncメソッドで存在するログインIDを指定するとユーザーを返す")]
    public async Task FindByLoginIdAsync_WithExistingLoginId_ReturnsUser()
    {
        // arrange
        const string loginId = "user001";

        // act
        var user = await _repository.FindByLoginIdAsync(loginId);

        // assert
        Assert.NotNull(user);
        Assert.Equal(loginId, user!.LoginId);
        Assert.Equal("ユーザー 太郎", user.DisplayName);
        Assert.Equal("user001@example.com", user.Email);
        Assert.Equal(UserRole.User, user.Role);
        Assert.True(user.IsActive);
    }

    [Fact]
    [DisplayName("FindByUserIdAsyncメソッドで存在しないユーザーIDを指定するとnullを返す")]
    public async Task FindByUserIdAsync_WithMissingUserId_ReturnsNull()
    {
        // arrange
        var userId = UserId.New();

        // act
        var user = await _repository.FindByUserIdAsync(userId);

        // assert
        Assert.Null(user);
    }
}
