using System.ComponentModel;
using StudySession.Domain.Users;

namespace StudySession.DomainTests;

public class UserIdTests
{
    [Fact]
    [DisplayName("Fromメソッドで空のGuidを渡すと例外が発生する")]
    public void From_WithEmptyGuid_ThrowsArgumentException()
    {
        // arrange
        Guid emptyGuid = Guid.Empty;

        // act % assert
        Assert.Throws<ArgumentException>(() => UserId.From(emptyGuid));
    }

    [Fact]
    [DisplayName("Newメソッドで生成されるUserIdは空のGuidではない")]
    public void New_CreatesNonEmptyUserId()
    {
        // arrange & act
        var userId = UserId.New();

        // assert
        Assert.NotEqual(Guid.Empty, userId.Value);
    }

    [Fact]
    [DisplayName("ToStringメソッドは内部のGuid値を返す")]
    public void ToString_ReturnsUnderlyingGuidValue()
    {
        // arrange
        var value = Guid.NewGuid();

        // act
        var userId = UserId.From(value);

        // assert
        Assert.Equal(value.ToString(), userId.ToString());
    }
}
