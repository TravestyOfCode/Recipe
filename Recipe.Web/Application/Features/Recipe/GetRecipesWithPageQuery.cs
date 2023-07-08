using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.Recipe;

public class GetRecipesWithPageQuery : PageQuery, IRequest<Result<RecipeListModel>>
{
    [BindNever]
    public string UserId { get; set; }

    public GetRecipesWithPageQuery()
    {
        Page = 1;

        PerPage = 10;
    }
}

public class GetRecipesWithPageQueryHandler : IRequestHandler<GetRecipesWithPageQuery, Result<RecipeListModel>>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<GetRecipesWithPageQuery> logger;

    public GetRecipesWithPageQueryHandler(AppDbContext dbContext, ILogger<GetRecipesWithPageQuery> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result<RecipeListModel>> Handle(GetRecipesWithPageQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await dbContext.Recipes.Where(p => p.UserId.Equals(request.UserId))
                .AsPageQuery(request)
                .Select(p => new RecipeModel()
                {
                    Categories = p.Categories.Select(c => new Category.CategoryModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList(),
                    Description = p.Description,
                    Id = p.Id,
                    Title = p.Title
                }).ToListAsync(cancellationToken);

            double count = await dbContext.Recipes.Where(p => p.UserId.Equals(request.UserId))
                .CountAsync(cancellationToken);

            return Result.Ok(new RecipeListModel(entities, request, count));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError<RecipeListModel>();
        }
    }
}
