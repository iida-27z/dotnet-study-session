using StudySession.Application.Abstractions.Security;

namespace StudySession.Infrastructure.Security;

/// <summary>
/// BCrypt implementation of password hashing service.
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    /// <inheritdoc/>
    public string Hash(string plainTextPassword)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
        {
            throw new ArgumentException("Password is required.", nameof(plainTextPassword));
        }

        return BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
    }

    /// <inheritdoc/>
    public bool Verify(string plainTextPassword, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(plainTextPassword, passwordHash);
    }
}
