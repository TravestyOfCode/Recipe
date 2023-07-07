using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;

namespace Recipe.Web.Application.Features.Recipe;

public class DeleteRecipeCommand : IRequest<Result>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class DeleteRecipeCommandHandler : IRequestHandler<DeleteRecipeCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<DeleteRecipeCommandHandler> logger;

    public DeleteRecipeCommandHandler(AppDbContext dbContext, ILogger<DeleteRecipeCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Recipes.SingleOrDefaultAsync(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId), cancellationToken);

            if (entity == null)
            {
                return Result.NotFound();
            }

            dbContext.Recipes.Remove(entity);

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
