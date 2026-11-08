using StudySessionEntity = StudySession.Domain.StudySessions.StudySession;

namespace StudySession.Application.StudySessions;

/// <summary>
/// Represents the result of a study session creation attempt.
/// </summary>
public sealed record CreateStudySessionResult(
    bool IsSuccess,
    StudySessionEntity? StudySession,
    IReadOnlyDictionary<string, string> Errors)
{
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static CreateStudySessionResult Success(StudySessionEntity studySession)
    {
        ArgumentNullException.ThrowIfNull(studySession);

        return new CreateStudySessionResult(true, studySession, new Dictionary<string, string>());
    }

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public static CreateStudySessionResult Failed(params (string FieldName, string Message)[] errors)
    {
        var dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (fieldName, message) in errors)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                continue;
            }

            dictionary[fieldName] = message;
        }

        return new CreateStudySessionResult(false, null, dictionary);
    }
}
