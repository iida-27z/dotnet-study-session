using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudySession.Application.StudySessions;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;

namespace StudySession.Web.Pages.StudySessions;

/// <summary>
/// Study session creation page.
/// </summary>
[Authorize]
public sealed class CreateModel : PageModel
{
    private readonly CreateStudySessionUseCase _createStudySessionUseCase;
    private readonly StudySessionTimeZone _studySessionTimeZone;

    public CreateModel(CreateStudySessionUseCase createStudySessionUseCase, StudySessionTimeZone studySessionTimeZone)
    {
        _createStudySessionUseCase = createStudySessionUseCase ?? throw new ArgumentNullException(nameof(createStudySessionUseCase));
        _studySessionTimeZone = studySessionTimeZone ?? throw new ArgumentNullException(nameof(studySessionTimeZone));
    }

    [BindProperty]
    [Display(Name = "勉強会テーマ")]
    [Required(ErrorMessage = "勉強会テーマは必須です。")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "勉強会テーマは1〜200文字で入力してください。")]
    public string Title { get; set; } = string.Empty;

    [BindProperty]
    [Display(Name = "開始日時")]
    [Required(ErrorMessage = "開始日時は必須です。")]
    public DateTime? StartTime { get; set; }

    [BindProperty]
    public string? StartTimeUtcValue { get; set; }

    [BindProperty]
    [Display(Name = "終了日時")]
    [Required(ErrorMessage = "終了日時は必須です。")]
    public DateTime? EndTime { get; set; }

    [BindProperty]
    public string? EndTimeUtcValue { get; set; }

    [BindProperty]
    [Display(Name = "開催形式")]
    [Required(ErrorMessage = "開催形式は必須です。")]
    public string? Format { get; set; } = StudySessionFormat.Hybrid.ToString();

    [BindProperty]
    [Display(Name = "対面枠定員")]
    [Range(0, 200, ErrorMessage = "対面枠定員は0〜200で入力してください。")]
    public int CapacityOnsite { get; set; } = 20;

    [BindProperty]
    [Display(Name = "リモート枠定員")]
    [Range(0, 500, ErrorMessage = "リモート枠定員は0〜500で入力してください。")]
    public int CapacityRemote { get; set; } = 30;

    [BindProperty]
    [Display(Name = "概要")]
    [StringLength(2000, ErrorMessage = "概要は2000文字以内で入力してください。")]
    public string? Description { get; set; }

    /// <summary>
    /// Handles GET requests.
    /// </summary>
    public IActionResult OnGet()
    {
        return Page();
    }

    /// <summary>
    /// Handles POST requests.
    /// </summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!TryParseFormat(out var format))
        {
            ModelState.AddModelError(nameof(Format), "開催形式が正しくありません。");
            return Page();
        }

        if (StartTime is null || EndTime is null)
        {
            return Page();
        }

        var startTimeUtc = ResolveUtcFromBrowserInput(StartTime, StartTimeUtcValue);
        var endTimeUtc = ResolveUtcFromBrowserInput(EndTime, EndTimeUtcValue);

        if (startTimeUtc < _studySessionTimeZone.UtcNow)
        {
            ModelState.AddModelError(nameof(StartTime), "開始日時は現在以降で入力してください。");
        }

        if (endTimeUtc <= startTimeUtc)
        {
            ModelState.AddModelError(nameof(EndTime), "終了日時は開始日時より後にしてください。");
        }

        switch (format)
        {
            case StudySessionFormat.OnsiteOnly when CapacityRemote != 0:
                ModelState.AddModelError(nameof(CapacityRemote), "対面のみの開催ではリモート枠は0にしてください。");
                break;
            case StudySessionFormat.RemoteOnly when CapacityOnsite != 0:
                ModelState.AddModelError(nameof(CapacityOnsite), "リモートのみの開催では対面枠は0にしてください。");
                break;
            case StudySessionFormat.Hybrid when CapacityOnsite < 1 || CapacityRemote < 1:
                ModelState.AddModelError(nameof(CapacityOnsite), "ハイブリッド開催では対面枠とリモート枠を1以上にしてください。");
                ModelState.AddModelError(nameof(CapacityRemote), "ハイブリッド開催では対面枠とリモート枠を1以上にしてください。");
                break;
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userIdValue))
        {
            return Forbid();
        }

        var request = new CreateStudySessionRequest(
            Title.Trim(),
            startTimeUtc,
            endTimeUtc,
            format,
            CapacityOnsite,
            CapacityRemote,
            Description?.Trim(),
            UserId.From(userIdValue));

        var result = await _createStudySessionUseCase.ExecuteAsync(request, cancellationToken);
        if (!result.IsSuccess)
        {
            foreach (var (fieldName, errorMessage) in result.Errors)
            {
                ModelState.AddModelError(fieldName, errorMessage);
            }

            return Page();
        }

        return RedirectToPage("/StudySessions/List");
    }

    private bool TryParseFormat(out StudySessionFormat format)
    {
        if (string.IsNullOrWhiteSpace(Format))
        {
            format = default;
            return false;
        }

        return Enum.TryParse<StudySessionFormat>(Format, true, out format)
            && Enum.IsDefined(typeof(StudySessionFormat), format);
    }

    private DateTime ResolveUtcFromBrowserInput(DateTime? localValue, string? isoUtcValue)
    {
        if (localValue is null)
        {
            throw new InvalidOperationException("日時が未設定です。");
        }

        if (!string.IsNullOrWhiteSpace(isoUtcValue)
            && DateTimeOffset.TryParse(isoUtcValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var exactUtc))
        {
            return exactUtc.UtcDateTime;
        }

        return _studySessionTimeZone.ToUtcFromBrowserJst(localValue.Value);
    }
}
