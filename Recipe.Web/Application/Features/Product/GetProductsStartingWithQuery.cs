using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.Product;

public class GetProductsStartingWithQuery : PageQuery, IRequest<Result<ProductListModel>>
{
    public string Name { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class GetProductsStartingWithQueryHandler : IRequestHandler<GetProductsStartingWithQuery, Result<ProductListModel>>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<GetProductsStartingWithQueryHandler> logger;

    public GetProductsStartingWithQueryHandler(AppDbContext dbContext, ILogger<GetProductsStartingWithQueryHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result<ProductListModel>> Handle(GetProductsStartingWithQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await dbContext.Products
                .Where(p => p.Name.StartsWith(request.Name) && p.UserId.Equals(request.UserId))
                .AsPageQuery(request)
                .Select(p => new ProductModel()
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync(cancellationToken);

            double count = await dbContext.Products
                .Where(p => p.Name.StartsWith(request.Name) && p.UserId.Equals(request.UserId))
                .CountAsync(cancellationToken);

            return Result.Ok(new ProductListModel(entities, request, count));

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError<ProductListModel>();
        }
    }
}
