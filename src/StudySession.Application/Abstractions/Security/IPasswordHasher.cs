namespace StudySession.Application.Abstractions.Security;

/// <summary>
/// Provides password hashing and verification operations.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Verifies a plain text password against a stored hash.
    /// </summary>
    bool Verify(string plainTextPassword, string passwordHash);

    /// <summary>
    /// Hashes a plain text password.
    /// </summary>
    string Hash(string plainTextPassword);
}
