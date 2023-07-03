using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipe.Web.Application.Features.Account;

namespace Recipe.Web.Areas.Account.Controllers;

[AllowAnonymous]
[Area("Account")]
public class RegisterController : Controller
{
    private readonly IMediator mediator;

    public RegisterController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(RegisterAccountCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return RedirectToAction(nameof(ConfirmEmail), routeValues: new { request.Email });
        }

        if (result.IsBadRequest)
        {
            return View();
        }

        return StatusCode(result.StatusCode);
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId != null && request.ConfirmationCode != null)
        {
            var result = await mediator.Send(request, cancellationToken);

            if (result.WasSuccessful)
            {
                return View(new ConfirmEmailModel(request.Email, true));
            }

            if (result.IsBadRequest)
            {
                ModelState.AddErrors(result);
            }
        }

        return View(new ConfirmEmailModel(request.Email, false));
    }
}
