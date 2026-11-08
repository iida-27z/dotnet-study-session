namespace StudySession.Domain.Users;

/// <summary>
/// Represents an authenticated user account.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    public User(
        UserId userId,
        string loginId,
        string displayName,
        string email,
        string passwordHash,
        UserRole role,
        bool isActive)
    {
        ArgumentNullException.ThrowIfNull(userId);

        LoginId = NormalizeRequiredString(loginId, 64, nameof(loginId));
        DisplayName = NormalizeRequiredString(displayName, 100, nameof(displayName));
        Email = NormalizeRequiredString(email, 254, nameof(email));
        PasswordHash = NormalizeRequiredString(passwordHash, 255, nameof(passwordHash));

        UserId = userId;
        Role = role;
        IsActive = isActive;
    }

    /// <summary>
    /// Gets the user ID.
    /// </summary>
    public UserId UserId { get; }

    /// <summary>
    /// Gets the login ID.
    /// </summary>
    public string LoginId { get; }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the email address.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Gets the bcrypt password hash.
    /// </summary>
    public string PasswordHash { get; }

    /// <summary>
    /// Gets the role.
    /// </summary>
    public UserRole Role { get; }

    /// <summary>
    /// Gets a value indicating whether the account can sign in.
    /// </summary>
    public bool IsActive { get; }

    /// <summary>
    /// Determines whether this account is allowed to sign in.
    /// </summary>
    public bool CanSignIn()
    {
        return IsActive;
    }

    private static string NormalizeRequiredString(string? value, int maxLength, string paramName)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Value is required.", paramName);
        }

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"Value must be <= {maxLength} characters.", paramName);
        }

        return normalized;
    }
}
