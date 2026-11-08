namespace StudySession.Application.Authentication;

/// <summary>
/// Represents an authentication request using login ID and password.
/// </summary>
public sealed record LoginRequest(string LoginId, string Password);
