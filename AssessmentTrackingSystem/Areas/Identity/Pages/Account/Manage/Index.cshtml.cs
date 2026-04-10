#nullable disable

using System.ComponentModel.DataAnnotations;
using ATS.Core.Entities;
using AssessmentTrackingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssessmentTrackingSystem.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class IndexModel : PageModel
{
    private static readonly IReadOnlyList<string> PresetAvatarSeeds =
    [
        "study-buddy",
        "campus-vibes",
        "late-night-lab",
        "quiz-master",
        "coffee-cram",
        "open-source",
        "deadline-radar",
        "math-mode",
        "library-lane",
        "code-sprint",
        "team-project",
        "focus-mode"
    ];

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyList<string> AvatarSeeds => PresetAvatarSeeds;

    public string AvatarPreviewUrl => AvatarUrlBuilder.Build(Input.AvatarSeed);
    public class InputModel
    {
        [Required]
        [Display(Name = "Display name")]
        public string DisplayName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number (optional)")]
        public string PhoneNumber { get; set; }

        [Required]
        public string AvatarSeed { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return NotFound("Unable to load the current user.");
        }

        await LoadAsync(user);
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
            Input.Email = await _userManager.GetEmailAsync(user);
            Input.DisplayName ??= ResolveDisplayName(user);
            Input.AvatarSeed ??= ResolveAvatarSeed(user);
            return Page();
        }

        var userUpdated = false;

        var selectedDisplayName = Input.DisplayName.Trim();
        if (!string.Equals(user.FirstName, selectedDisplayName, StringComparison.Ordinal))
        {
            user.FirstName = selectedDisplayName;
            userUpdated = true;
        }

        var currentPhoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (Input.PhoneNumber != currentPhoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "There was a problem saving your phone number.";
                return RedirectToPage();
            }
        }

        var selectedSeed = Input.AvatarSeed.Trim();
        if (!string.Equals(user.AvatarSeed, selectedSeed, StringComparison.Ordinal))
        {
            user.AvatarSeed = selectedSeed;
            userUpdated = true;
        }

        if (userUpdated)
        {
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                StatusMessage = "There was a problem saving your profile.";
                return RedirectToPage();
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated.";
        return RedirectToPage();
    }

    private async Task LoadAsync(ApplicationUser user)
    {
        Input = new InputModel
        {
            DisplayName = ResolveDisplayName(user),
            Email = await _userManager.GetEmailAsync(user),
            PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
            AvatarSeed = ResolveAvatarSeed(user)
        };
    }

    private static string ResolveDisplayName(ApplicationUser user)
    {
        if (!string.IsNullOrWhiteSpace(user.FirstName))
        {
            return user.FirstName;
        }

        if (!string.IsNullOrWhiteSpace(user.UserName) && user.UserName.Contains('@'))
        {
            return user.UserName.Split('@')[0];
        }

        return string.IsNullOrWhiteSpace(user.UserName)
            ? "Student"
            : user.UserName;
    }

    private static string ResolveAvatarSeed(ApplicationUser user)
    {
        if (!string.IsNullOrWhiteSpace(user.AvatarSeed))
        {
            return user.AvatarSeed;
        }

        var fallbackSeed = $"{user.FirstName} {user.LastName}".Trim();
        if (!string.IsNullOrWhiteSpace(fallbackSeed))
        {
            return fallbackSeed;
        }

        return string.IsNullOrWhiteSpace(user.UserName)
            ? "study-buddy"
            : user.UserName;
    }
}
