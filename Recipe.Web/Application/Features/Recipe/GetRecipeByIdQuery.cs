using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Application.Features.Ingredient;
using Recipe.Web.Application.Features.UnitOfMeasure;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.Recipe;

public class GetRecipeByIdQuery : IRequest<Result<RecipeModel>>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }

    public bool IncludeIngredients { get; set; }
}

public class GetRecipeByIdQueryHandler : IRequestHandler<GetRecipeByIdQuery, Result<RecipeModel>>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<GetRecipeByIdQuery> logger;

    public GetRecipeByIdQueryHandler(AppDbContext dbContext, ILogger<GetRecipeByIdQuery> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result<RecipeModel>> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Recipes.Where(p => p.UserId.Equals(request.UserId) && p.Id.Equals(request.Id))
                .Select(p => new RecipeModel()
                {
                    Categories = p.Categories,
                    Description = p.Description,
                    Id = p.Id,
                    Ingredients = request.IncludeIngredients ? p.Ingredients.Select(i => new IngredientModel()
                    {
                        Id = i.Id,
                        Product = i.Product,
                        Quantity = i.Quantity,
                        RecipeId = i.RecipeId,
                        UnitOfMeasureId = i.UnitOfMeasureId,
                        UnitOfMeasure = i.UnitOfMeasure == null ? null : new UnitOfMeasureModel()
                        {
                            Abbreviation = i.UnitOfMeasure.Abbreviation,
                            ConversionToGramsRatio = i.UnitOfMeasure.ConversionToGramsRatio,
                            Id = i.UnitOfMeasure.Id,
                            Name = i.UnitOfMeasure.Name
                        }
                    }).ToList() : null,
                    Instructions = p.Instructions,
                    Title = p.Title
                }).SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return Result.NotFound<RecipeModel>();
            }

            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError<RecipeModel>();
        }
    }
}
