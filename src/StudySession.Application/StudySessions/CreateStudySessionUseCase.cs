using StudySession.Domain.StudySessions;
using StudySessionEntity = StudySession.Domain.StudySessions.StudySession;

namespace StudySession.Application.StudySessions;

/// <summary>
/// Handles validation and persistence for creating a study session.
/// </summary>
public sealed class CreateStudySessionUseCase
{
    private readonly IStudySessionRepository _studySessionRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateStudySessionUseCase"/> class.
    /// </summary>
    public CreateStudySessionUseCase(IStudySessionRepository studySessionRepository)
    {
        _studySessionRepository = studySessionRepository;
    }

    /// <summary>
    /// Validates and persists a study session.
    /// </summary>
    public async Task<CreateStudySessionResult> ExecuteAsync(CreateStudySessionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var errors = new List<(string FieldName, string Message)>();

        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length is < 1 or > 200)
        {
            errors.Add((nameof(request.Title), "勉強会テーマは1〜200文字で入力してください。"));
        }

        if (request.StartTime < DateTime.Now)
        {
            errors.Add((nameof(request.StartTime), "開始日時は現在以降で入力してください。"));
        }

        if (request.EndTime <= request.StartTime)
        {
            errors.Add((nameof(request.EndTime), "終了日時は開始日時より後にしてください。"));
        }

        if (!Enum.IsDefined(typeof(StudySessionFormat), request.Format))
        {
            errors.Add((nameof(request.Format), "開催形式が正しくありません。"));
        }

        if (request.CapacityOnsite is < 0 or > 200)
        {
            errors.Add((nameof(request.CapacityOnsite), "対面枠定員は0〜200で入力してください。"));
        }

        if (request.CapacityRemote is < 0 or > 500)
        {
            errors.Add((nameof(request.CapacityRemote), "リモート枠定員は0〜500で入力してください。"));
        }

        if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Length > 2000)
        {
            errors.Add((nameof(request.Description), "概要は2000文字以内で入力してください。"));
        }

        switch (request.Format)
        {
            case StudySessionFormat.OnsiteOnly when request.CapacityRemote != 0:
                errors.Add((nameof(request.CapacityRemote), "対面のみの開催ではリモート枠は0にしてください。"));
                break;
            case StudySessionFormat.RemoteOnly when request.CapacityOnsite != 0:
                errors.Add((nameof(request.CapacityOnsite), "リモートのみの開催では対面枠は0にしてください。"));
                break;
            case StudySessionFormat.Hybrid when request.CapacityOnsite < 1 || request.CapacityRemote < 1:
                errors.Add((nameof(request.CapacityOnsite), "ハイブリッド開催では対面枠とリモート枠を1以上にしてください。"));
                errors.Add((nameof(request.CapacityRemote), "ハイブリッド開催では対面枠とリモート枠を1以上にしてください。"));
                break;
        }

        if (errors.Count > 0)
        {
            return CreateStudySessionResult.Failed([.. errors]);
        }

        var now = DateTime.UtcNow;
        var studySession = new StudySessionEntity(
            StudySessionId.New(),
            request.Title.Trim(),
            request.Description?.Trim() ?? string.Empty,
            request.StartTime,
            request.EndTime,
            request.Format,
            request.CapacityOnsite,
            request.CapacityRemote,
            request.CreatorUserId,
            now,
            now);

        await _studySessionRepository.AddAsync(studySession, cancellationToken);

        return CreateStudySessionResult.Success(studySession);
    }
}
