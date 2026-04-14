#nullable disable

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssessmentTrackingSystem.Areas.Identity.Pages.Account;

public class RegisterConfirmationModel : PageModel
{
    public string Email { get; private set; }
    public string ReturnUrl { get; private set; }

    public void OnGet(string email, string returnUrl = null)
    {
        Email = email;
        ReturnUrl = returnUrl;
    }
}
