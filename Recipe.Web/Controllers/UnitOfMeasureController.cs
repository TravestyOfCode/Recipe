using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipe.Web.Application.Features.UnitOfMeasure;
using System.Security.Claims;

namespace Recipe.Web.Controllers;

[Authorize]
public class UnitOfMeasureController : Controller
{
    private readonly IMediator mediator;

    public UnitOfMeasureController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(GetUnitOfMeasuresWithPageQuery request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return View(result.Value);
        }

        return StatusCode(result.StatusCode);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return RedirectToAction(nameof(Index));
        }

        if (result.IsBadRequest)
        {
            ModelState.AddErrors(result);

            return View();
        }

        return StatusCode(result.StatusCode);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(GetUnitOfMeasureByIdQuery request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return View(result.Value);
        }

        return StatusCode(result.StatusCode);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return RedirectToAction(nameof(Index));
        }

        if (result.IsBadRequest)
        {
            ModelState.AddErrors(result);

            return View();
        }

        return StatusCode(result.StatusCode);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(GetUnitOfMeasureByIdQuery request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return View(result.Value);
        }

        return StatusCode(result.StatusCode);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(DeleteUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return RedirectToAction(nameof(Index));
        }

        return StatusCode(result.StatusCode);
    }
}
