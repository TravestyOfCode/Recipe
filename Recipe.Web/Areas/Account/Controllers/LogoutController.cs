using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Recipe.Web.Data;

namespace Recipe.Web.Areas.Account.Controllers;

[Authorize]
[Area("Account")]
public class LogoutController : Controller
{
    private readonly SignInManager<AppUser> signInManager;

    public LogoutController(SignInManager<AppUser> signInManager)
    {
        this.signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();

        return LocalRedirect("~/");
    }
}
