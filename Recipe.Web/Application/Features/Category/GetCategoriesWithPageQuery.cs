using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.Category;

public class GetCategoriesWithPageQuery : PageQuery, IRequest<Result<CategoryListModel>>
{
    [BindNever]
    public string UserId { get; set; }
}

public class GetCategoriesWithPageQueryHandler : IRequestHandler<GetCategoriesWithPageQuery, Result<CategoryListModel>>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<GetCategoriesWithPageQueryHandler> logger;

    public GetCategoriesWithPageQueryHandler(AppDbContext dbContext, ILogger<GetCategoriesWithPageQueryHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result<CategoryListModel>> Handle(GetCategoriesWithPageQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await dbContext.Categories
                .Where(p => p.UserId.Equals(request.UserId))
                .AsPageQuery(request)
                .Select(p => new CategoryModel()
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToListAsync(cancellationToken);

            double count = await dbContext.Categories
                .Where(p => p.UserId.Equals(request.UserId))
                .CountAsync(cancellationToken);

            return Result.Ok(new CategoryListModel(entities, request, count));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError<CategoryListModel>();
        }
    }
}

public class GetCategoriesWithPageQueryValdiator : AbstractValidator<GetCategoriesWithPageQuery>
{
    public GetCategoriesWithPageQueryValdiator()
    {
        Include(new PageQueryValidator());
    }
}
