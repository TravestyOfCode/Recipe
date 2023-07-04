using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipe.Web.Application.Features.Category;
using System.Security.Claims;

namespace Recipe.Web.Controllers;

[Authorize]
public class CategoryController : Controller
{
    private readonly IMediator mediator;

    public CategoryController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(GetCategoriesWithPageQuery request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Create(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

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
    public async Task<IActionResult> Edit(GetCategoryByIdQuery request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Edit(EditCategoryCommand request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Delete(GetCategoryByIdQuery request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Delete(DeleteCategoryCommand request, CancellationToken cancellationToken)
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
