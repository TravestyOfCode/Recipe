using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.Product;

public class GetProductByIdQuery : IRequest<Result<ProductModel>>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductModel>>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<GetProductByIdQueryHandler> logger;

    public GetProductByIdQueryHandler(AppDbContext dbContext, ILogger<GetProductByIdQueryHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result<ProductModel>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Products
                .Where(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId))
                .Select(p => new ProductModel()
                {
                    Id = p.Id,
                    Name = p.Name
                }).SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return Result.NotFound<ProductModel>();
            }

            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError<ProductModel>();
        }
    }
}
