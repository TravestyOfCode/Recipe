using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;

namespace Recipe.Web.Application.Features.Category;

public class DeleteCategoryCommand : IRequest<Result>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<DeleteCategoryCommandHandler> logger;

    public DeleteCategoryCommandHandler(AppDbContext dbContext, ILogger<DeleteCategoryCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Categories
                .SingleOrDefaultAsync(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId), cancellationToken);

            if (entity == null)
            {
                return Result.NotFound();
            }

            dbContext.Categories.Remove(entity);

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
