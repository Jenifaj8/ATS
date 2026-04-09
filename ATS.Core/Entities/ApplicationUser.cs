using Microsoft.AspNetCore.Identity;

namespace ATS.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarSeed { get; set; }
}
