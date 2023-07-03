using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipe.Web.Application.Features.Account;

namespace Recipe.Web.Areas.Account.Controllers;

[AllowAnonymous]
[Area("Account")]
public class LoginController : Controller
{
    private readonly IMediator mediator;

    public LoginController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginCommand request, string returnUrl, CancellationToken cancellationToken)
    {
        returnUrl ??= Url.Content("~/");

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return LocalRedirect(returnUrl);
        }

        if (result.IsBadRequest)
        {
            ModelState.AddErrors(result);

            return View();
        }

        return StatusCode(result.StatusCode);
    }
}

