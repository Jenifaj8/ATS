using System.Security.Claims;
using ATS.Core.Entities;
using ATS.Infrastructure.Data;
using AssessmentTrackingSystem.Areas.Identity.Pages.Account.Manage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ATS.Tests;

public class ProfileManageIndexTests
{
    [Fact]
    public async Task OnGetAsync_LoadsDisplayNameFromFirstName_WhenFirstNameExists()
    {
        await using var context = CreateContext();
        var userManager = CreateUserManager(context);
        var signInManager = CreateSignInManager(userManager);
        var user = await CreateUserAsync(userManager,
            firstName: "Ryland",
            lastName: "Palmier",
            userName: "haywire.palmier.0x@icloud.com",
            email: "haywire.palmier.0x@icloud.com",
            avatarSeed: "focus-mode");

        var pageModel = CreatePageModel(userManager, signInManager, user.Id);

        var result = await pageModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("Ryland", pageModel.Input.DisplayName);
        Assert.Equal("focus-mode", pageModel.Input.AvatarSeed);
    }

    [Fact]
    public async Task OnGetAsync_UsesEmailPrefix_WhenFirstNameIsEmpty()
    {
        await using var context = CreateContext();
        var userManager = CreateUserManager(context);
        var signInManager = CreateSignInManager(userManager);
        var user = await CreateUserAsync(userManager,
            firstName: null,
            lastName: "Palmier",
            userName: "haywire.palmier.0x@icloud.com",
            email: "haywire.palmier.0x@icloud.com",
            avatarSeed: "study-buddy");

        var pageModel = CreatePageModel(userManager, signInManager, user.Id);

        var result = await pageModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("haywire.palmier.0x", pageModel.Input.DisplayName);
    }

    [Fact]
    public async Task OnGetAsync_UsesNameFallback_WhenAvatarSeedIsEmpty()
    {
        await using var context = CreateContext();
        var userManager = CreateUserManager(context);
        var signInManager = CreateSignInManager(userManager);
        var user = await CreateUserAsync(userManager,
            firstName: "Ryland",
            lastName: "Palmier",
            userName: "haywire.palmier.0x@icloud.com",
            email: "haywire.palmier.0x@icloud.com",
            avatarSeed: null);

        var pageModel = CreatePageModel(userManager, signInManager, user.Id);

        var result = await pageModel.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal("Ryland Palmier", pageModel.Input.AvatarSeed);
    }

    [Fact]
    public async Task OnPostAsync_UpdatesFirstNameAndAvatarSeed_WhenValuesChange()
    {
        await using var context = CreateContext();
        var userManager = CreateUserManager(context);
        var signInManager = CreateSignInManager(userManager);
        var user = await CreateUserAsync(userManager,
            firstName: "Ryland",
            lastName: "Palmier",
            userName: "haywire.palmier.0x@icloud.com",
            email: "haywire.palmier.0x@icloud.com",
            avatarSeed: "study-buddy");

        var pageModel = CreatePageModel(userManager, signInManager, user.Id);
        pageModel.Input = new IndexModel.InputModel
        {
            DisplayName = "Rye",
            Email = "haywire.palmier.0x@icloud.com",
            PhoneNumber = null,
            AvatarSeed = "quiz-master"
        };

        var result = await pageModel.OnPostAsync();
        var updatedUser = await userManager.FindByIdAsync(user.Id);

        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Null(redirectResult.PageName);
        Assert.Equal("Rye", updatedUser?.FirstName);
        Assert.Equal("quiz-master", updatedUser?.AvatarSeed);
        Assert.Equal("Your profile has been updated.", pageModel.StatusMessage);
    }

    private static AtsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AtsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AtsDbContext(options);
    }

    private static UserManager<ApplicationUser> CreateUserManager(AtsDbContext context)
    {
        var store = new UserStore<ApplicationUser, IdentityRole, AtsDbContext, string>(context);
        var services = new ServiceCollection().BuildServiceProvider();

        return new UserManager<ApplicationUser>(
            store,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<ApplicationUser>(),
            [new UserValidator<ApplicationUser>()],
            [new PasswordValidator<ApplicationUser>()],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            services,
            LoggerFactory.Create(builder => { }).CreateLogger<UserManager<ApplicationUser>>());
    }

    private static TestSignInManager CreateSignInManager(UserManager<ApplicationUser> userManager)
    {
        return new TestSignInManager(
            userManager,
            new HttpContextAccessor(),
            new UserClaimsPrincipalFactory<ApplicationUser>(userManager, Options.Create(new IdentityOptions())),
            Options.Create(new IdentityOptions()),
            LoggerFactory.Create(builder => { }).CreateLogger<SignInManager<ApplicationUser>>(),
            new AuthenticationSchemeProvider(Options.Create(new AuthenticationOptions())),
            new DefaultUserConfirmation<ApplicationUser>());
    }

    private static IndexModel CreatePageModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        string userId)
    {
        var pageModel = new IndexModel(userManager, signInManager)
        {
            PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreatePrincipal(userId)
                }
            }
        };

        return pageModel;
    }

    private static ClaimsPrincipal CreatePrincipal(string userId)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId)
        ], "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private static async Task<ApplicationUser> CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string? firstName,
        string? lastName,
        string userName,
        string email,
        string? avatarSeed)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = firstName,
            LastName = lastName,
            UserName = userName,
            Email = email,
            AvatarSeed = avatarSeed
        };

        var result = await userManager.CreateAsync(user);
        Assert.True(result.Succeeded);

        return user;
    }

    private sealed class TestSignInManager : SignInManager<ApplicationUser>
    {
        public TestSignInManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public override Task RefreshSignInAsync(ApplicationUser user)
        {
            return Task.CompletedTask;
        }
    }
}
