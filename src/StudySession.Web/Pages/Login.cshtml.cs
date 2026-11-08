using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudySession.Application.Authentication;

namespace StudySession.Web.Pages;

/// <summary>
/// Login page model.
/// </summary>
[AllowAnonymous]
public sealed class LoginModel : PageModel
{
    private readonly LoginUseCase _loginUseCase;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginModel"/> class.
    /// </summary>
    public LoginModel(LoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }

    /// <summary>
    /// Gets or sets login ID input.
    /// </summary>
    [BindProperty]
    [Display(Name = "ユーザーID")]
    [Required(ErrorMessage = "ユーザーIDは必須です。")]
    [StringLength(64, ErrorMessage = "ユーザーIDは64文字以内で入力してください。")]
    public string LoginId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets password input.
    /// </summary>
    [BindProperty]
    [Display(Name = "パスワード")]
    [Required(ErrorMessage = "パスワードは必須です。")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets error message shown on authentication failure.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Handles GET requests.
    /// </summary>
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/StudySessions/List");
        }

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

        var result = await _loginUseCase.ExecuteAsync(new LoginRequest(LoginId, Password), cancellationToken);
        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage;
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.UserId!.ToString()),
            new(ClaimTypes.Name, result.DisplayName!),
            new(ClaimTypes.Role, result.Role!),
            new("login_id", result.LoginId!),
        };

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow,
            });

        return RedirectToPage("/StudySessions/List");
    }
}
