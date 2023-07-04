using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;

namespace Recipe.Web.Application.Features.Product;

public class DeleteProductCommand : IRequest<Result>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<DeleteProductCommandHandler> logger;

    public DeleteProductCommandHandler(AppDbContext dbContext, ILogger<DeleteProductCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Products
                .SingleOrDefaultAsync(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId), cancellationToken);

            if (entity == null)
            {
                return Result.NotFound();
            }

            dbContext.Products.Remove(entity);

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError();
        }
    }
}

