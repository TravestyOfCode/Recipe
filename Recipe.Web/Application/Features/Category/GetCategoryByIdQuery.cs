using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.Category;

public class GetCategoryByIdQuery : IRequest<Result<CategoryModel>>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryModel>>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<GetCategoryByIdQueryHandler> logger;

    public GetCategoryByIdQueryHandler(AppDbContext dbContext, ILogger<GetCategoryByIdQueryHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result<CategoryModel>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Categories
                .Where(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId))
                .Select(p => new CategoryModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                }).SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return Result.NotFound<CategoryModel>();
            }

            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError<CategoryModel>();
        }
    }
}
