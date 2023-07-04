using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.Product;

public class GetProductsWithPageQuery : PageQuery, IRequest<Result<ProductListModel>>
{
    [BindNever]
    public string UserId { get; set; }
}

public class GetProductsWithPageQueryHandler : IRequestHandler<GetProductsWithPageQuery, Result<ProductListModel>>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<GetProductsWithPageQueryHandler> logger;

    public GetProductsWithPageQueryHandler(AppDbContext dbContext, ILogger<GetProductsWithPageQueryHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result<ProductListModel>> Handle(GetProductsWithPageQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await dbContext.Products.Where(p => p.UserId.Equals(request.UserId))
                .AsPageQuery(request)
                .Select(p => new ProductModel()
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToListAsync(cancellationToken);

            double totalCount = await dbContext.UnitOfMeasures.Where(p => p.UserId.Equals(request.UserId))
                .CountAsync(cancellationToken);

            return Result.Ok(new ProductListModel(entities, request, totalCount));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError<ProductListModel>();
        }
    }
}

public class GetProductsWithPageQueryValidator : AbstractValidator<GetProductsWithPageQuery>
{
    public GetProductsWithPageQueryValidator()
    {
        Include(new PageQueryValidator());
    }
}
