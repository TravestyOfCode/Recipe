using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Application.Features.Ingredient;
using Recipe.Web.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Recipe.Web.Application.Features.Recipe;

public class CreateRecipeCommand : IRequest<Result>
{
    [BindNever]
    public string UserId { get; set; }

    [Required]
    [MaxLength(64)]
    public string Title { get; set; }

    //public string Categories { get; set; }

    public List<string> Categories { get; set; }

    public string Description { get; set; }

    public List<IngredientModel> Ingredients { get; set; } = new List<IngredientModel>();

    public string Instructions { get; set; }
}

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<CreateRecipeCommandHandler> logger;

    public CreateRecipeCommandHandler(AppDbContext dbContext, ILogger<CreateRecipeCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ingredients = await ToIngredientList(request.Ingredients, request.UserId, cancellationToken);

            var entity = dbContext.Recipes.Add(new Data.Recipe()
            {
                Categories = await ToCategoryList(request.Categories, request.UserId, cancellationToken),
                Description = request.Description,
                Ingredients = ingredients,
                Instructions = request.Instructions,
                Title = request.Title,
                UserId = request.UserId
            });

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

            entity ??= new Data.Category()
            {
                Name = category,
                UserId = userId
            };

            results.Add(entity);
        }

        return results;
    }

    private async Task<List<Data.Ingredient>> ToIngredientList(IEnumerable<IngredientModel> ingredients, string userId, CancellationToken cancellationToken)
    {
        var results = new List<Data.Ingredient>();

        foreach (var ingredient in ingredients)
        {
            var product = await dbContext.Products
                .AsTracking()
                .Where(p => p.Name.Equals(ingredient.Product) && p.UserId.Equals(userId))
                .SingleOrDefaultAsync(cancellationToken);

            results.Add(new Data.Ingredient()
            {
                Product = product ?? new Data.Product()
                {
                    Name = ingredient.Product,
                    UserId = userId
                },
                Quantity = ingredient.Quantity,
                UnitOfMeasureId = ingredient.UnitOfMeasureId
            });
        }

        return results;
    }
}
