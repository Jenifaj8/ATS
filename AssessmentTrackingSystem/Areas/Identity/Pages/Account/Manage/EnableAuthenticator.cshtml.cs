#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using ATS.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssessmentTrackingSystem.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class EnableAuthenticatorModel : PageModel
{
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UrlEncoder _urlEncoder;

    public EnableAuthenticatorModel(UserManager<ApplicationUser> userManager, UrlEncoder urlEncoder)
    {
        _userManager = userManager;
        _urlEncoder = urlEncoder;
    }

    [TempData]
    public string StatusMessage { get; set; }

    public string SharedKey { get; set; }
    public string AuthenticatorUri { get; set; }
    public bool IsTwoFactorEnabled { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The verification code must be between {2} and {1} characters.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification code")]
        public string Code { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound("Unable to load the current user.");
        }

        await LoadSharedKeyAndQrCodeUriAsync(user);
        IsTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound("Unable to load the current user.");
        }

        if (!ModelState.IsValid)
        {
            await LoadSharedKeyAndQrCodeUriAsync(user);
            IsTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            return Page();
        }

        var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
        var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            verificationCode);

        if (!is2faTokenValid)
        {
            ModelState.AddModelError("Input.Code", "The verification code is invalid.");
            await LoadSharedKeyAndQrCodeUriAsync(user);
            IsTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            return Page();
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        StatusMessage = "Your authenticator app has been verified and two-factor authentication is enabled.";

        return RedirectToPage("./Index");
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
    {
        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        SharedKey = FormatKey(unformattedKey);
        var email = await _userManager.GetEmailAsync(user) ?? user.UserName ?? "student";
        AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            AuthenticatorUriFormat,
            _urlEncoder.Encode("AssessmentTrackingSystem"),
            _urlEncoder.Encode(email),
            unformattedKey);
    }

    private static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        var currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }

        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }
}
