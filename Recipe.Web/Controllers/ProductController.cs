using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipe.Web.Application.Features.Product;
using System.Security.Claims;

namespace Recipe.Web.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly IMediator mediator;

    public ProductController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(GetProductsWithPageQuery request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return View(result.Value);
        }

        if (result.IsBadRequest)
        {
            ModelState.AddErrors(result);

            return View();
        }

        return StatusCode(result.StatusCode);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Edit(GetProductByIdQuery request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Edit(EditProductCommand request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Delete(GetProductByIdQuery request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Delete(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return RedirectToAction(nameof(Index));
        }

        return StatusCode(result.StatusCode);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductsByName(GetProductsStartingWithQuery request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return Ok(result.Value);
        }

        return StatusCode(result.StatusCode);
    }
}
