using StudySession.Domain.Users;

namespace StudySession.Application.Authentication;

/// <summary>
/// Represents the result of a login attempt.
/// </summary>
public sealed record LoginResult(
    bool IsSuccess,
    Guid? UserId,
    string? LoginId,
    string? DisplayName,
    string? Role,
    string? ErrorMessage)
{
    /// <summary>
    /// Generic authentication error message.
    /// </summary>
    public const string AuthenticationFailedMessage = "ログインIDまたはパスワードが正しくありません。";

    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static LoginResult Success(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new LoginResult(
            IsSuccess: true,
            UserId: user.UserId,
            LoginId: user.LoginId,
            DisplayName: user.DisplayName,
            Role: user.Role.ToString(),
            ErrorMessage: null);
    }

    /// <summary>
    /// Creates a failure result with the standard error message.
    /// </summary>
    public static LoginResult Failed()
    {
        return new LoginResult(
            IsSuccess: false,
            UserId: null,
            LoginId: null,
            DisplayName: null,
            Role: null,
            ErrorMessage: AuthenticationFailedMessage);
    }
}
