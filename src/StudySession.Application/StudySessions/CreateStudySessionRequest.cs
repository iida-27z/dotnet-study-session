using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;

namespace StudySession.Application.StudySessions;

/// <summary>
/// Represents a request to create a new study session.
/// </summary>
public sealed record CreateStudySessionRequest(
    string Title,
    DateTime StartTime,
    DateTime EndTime,
    StudySessionFormat Format,
    int CapacityOnsite,
    int CapacityRemote,
    string? Description,
    UserId CreatorUserId);
