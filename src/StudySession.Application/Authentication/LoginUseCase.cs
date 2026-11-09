using StudySession.Application.Abstractions.Security;
using StudySession.Domain.Users;

namespace StudySession.Application.Authentication;

/// <summary>
/// Handles authentication by login ID and password.
/// </summary>
public sealed class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginUseCase"/> class.
    /// </summary>
    public LoginUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Authenticates a user.
    /// </summary>
    public async Task<LoginResult> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.LoginId) || string.IsNullOrWhiteSpace(request.Password))
        {
            return LoginResult.Failed();
        }

        var normalizedLoginId = request.LoginId.Trim();
        var user = await _userRepository.FindByLoginIdAsync(normalizedLoginId, cancellationToken);

        if (user is null || !user.CanSignIn())
        {
            return LoginResult.Failed();
        }

        var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return LoginResult.Failed();
        }

        return LoginResult.Success(user);
    }
}
