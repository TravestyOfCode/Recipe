using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Application.Features.Ingredient;
using Recipe.Web.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Recipe.Web.Application.Features.Recipe;

public class EditRecipeCommand : IRequest<Result>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }

    [Required]
    [MaxLength(64)]
    public string Title { get; set; }

    public List<string> Categories { get; set; }

    public string Description { get; set; }

    public List<IngredientModel> Ingredients { get; set; } = new List<IngredientModel>();

    public string Instructions { get; set; }
}

public class EditRecipeCommandHandler : IRequestHandler<EditRecipeCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<EditRecipeCommandHandler> logger;

    public EditRecipeCommandHandler(AppDbContext dbContext, ILogger<EditRecipeCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(EditRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Recipes.Where(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId))
                .Include(p => p.Categories)
                .Include(p => p.Ingredients)
                .AsTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return Result.NotFound();
            }

            entity.Title = request.Title;
            entity.Categories = await ToCategoryList(request.Categories, request.UserId, cancellationToken);
            entity.Description = request.Description;
            entity.Instructions = request.Instructions;

            entity.Ingredients = new List<Data.Ingredient>();

            foreach (var ingredient in request.Ingredients)
            {
                if (ingredient.Id != 0)
                {
                    var entityIngredient = await dbContext.Ingredients.Where(p => p.Id.Equals(ingredient.Id) && p.RecipeId.Equals(request.Id))
                        .AsTracking()
                        .SingleOrDefaultAsync(cancellationToken);

                    if (entityIngredient == null)
                    {
                        logger.LogError("The ingredient id {id} in a recipe edit request was not found for recipe id {recipeId}", ingredient.Id, request.Id);

                        return Result.NotFound();
                    }

                    entityIngredient.Product = ingredient.Product;
                    entityIngredient.Quantity = ingredient.Quantity;
                    entityIngredient.UnitOfMeasureId = ingredient.UnitOfMeasureId;

                    entity.Ingredients.Add(entityIngredient);
                }
                else
                {
                    entity.Ingredients.Add(new Data.Ingredient()
                    {
                        Product = ingredient.Product,
                        Quantity = ingredient.Quantity,
                        RecipeId = ingredient.RecipeId,
                        UnitOfMeasureId = ingredient.UnitOfMeasureId
                    });
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError();
        }
    }

    private async Task<List<Data.Category>> ToCategoryList(IEnumerable<string> categories, string userId, CancellationToken cancellationToken)
    {
        List<Data.Category> results = new List<Data.Category>();

        foreach (var category in categories)
        {
            var entity = await dbContext.Categories
                .AsTracking()
                .Where(p => p.UserId.Equals(userId) && p.Name.Equals(category))
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                entity = new Data.Category()
                {
                    Name = category,
                    UserId = userId
                };
            }

            results.Add(entity);
        }

        return results;
    }
}

public class EditRecipeCommandValidator : AbstractValidator<EditRecipeCommand>
{
    public EditRecipeCommandValidator()
    {
        RuleForEach(p => p.Ingredients).ChildRules(i =>
        {
            i.RuleFor(p => p.Product).NotEmpty();
            i.RuleFor(p => p.Quantity).GreaterThan(0).PrecisionScale(9, 5, true);
        });

    }
}
