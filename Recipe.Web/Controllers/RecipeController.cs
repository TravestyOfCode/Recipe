using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipe.Web.Application.Features.Recipe;
using Recipe.Web.Application.Features.UnitOfMeasure;
using Recipe.Web.ViewModels.Ingredient;
using Recipe.Web.ViewModels.Recipe;
using System.Security.Claims;

namespace Recipe.Web.Controllers;

[Authorize]
public class RecipeController : Controller
{
    private readonly IMediator mediator;

    public RecipeController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(GetRecipesWithPageQuery request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var uoms = await mediator.Send(new GetUnitOfMeasuresAsDictionaryQuery(userId), cancellationToken);

        if (!uoms.WasSuccessful)
        {
            return StatusCode(uoms.StatusCode);
        }

        return View(new CreateRecipeModel(uoms.Value));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRecipeCommand request, CancellationToken cancellationToken)
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

            var uoms = await mediator.Send(new GetUnitOfMeasuresAsDictionaryQuery(request.UserId), cancellationToken);

            if (!uoms.WasSuccessful)
            {
                return StatusCode(uoms.StatusCode);
            }

            return View(new CreateRecipeModel(uoms.Value));
        }

        return StatusCode(result.StatusCode);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return View(await GenerateEditViewModel(result.Value, cancellationToken));
        }

        return StatusCode(result.StatusCode);
    }

    [HttpPost]
    public async Task<IActionResult> Edit([Bind(Prefix = "Recipe")] EditRecipeCommand request, CancellationToken cancellationToken)
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

            var model = new RecipeModel()
            {
                Ingredients = request.Ingredients
            };

            return View(await GenerateEditViewModel(model, cancellationToken));
        }

        return StatusCode(result.StatusCode);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(GetRecipeByIdQuery request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Delete(DeleteRecipeCommand request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> Details(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        request.IncludeIngredients = true;

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return View(result.Value);
        }

        return StatusCode(result.StatusCode);
    }

    [HttpGet]
    public async Task<IActionResult> AddIngredient(GetUnitOfMeasuresAsDictionaryQuery request, string prefix, CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await mediator.Send(request, cancellationToken);

        if (result.WasSuccessful)
        {
            var model = new IngredientAddViewModel
            {
                UnitOfMeasrues = result.Value,
                NamePrefix = prefix
            };

            return PartialView("AddIngredientPartial", model);
        }

        return StatusCode(result.StatusCode);
    }

    private async Task<RecipeEditViewModel> GenerateEditViewModel(RecipeModel recipe, CancellationToken cancellationToken)
    {
        var result = new RecipeEditViewModel() { Recipe = recipe };

        var uoms = await mediator.Send(new GetUnitOfMeasuresAsDictionaryQuery() { UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) }, cancellationToken);

        if (uoms.WasSuccessful)
        {
            result.UnitOfMeasures = uoms.Value;
        }

        return result;
    }
}
