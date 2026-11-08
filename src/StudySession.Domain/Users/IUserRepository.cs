using StudySession.Domain.Users;

namespace StudySession.Domain.Users;

/// <summary>
/// Provides user persistence operations needed by application use cases.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Finds a user by login ID.
    /// </summary>
    Task<User?> FindByLoginIdAsync(string loginId, CancellationToken cancellationToken = default);
}
