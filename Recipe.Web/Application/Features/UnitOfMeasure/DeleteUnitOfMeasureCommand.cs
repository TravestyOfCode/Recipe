using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;

namespace Recipe.Web.Application.Features.UnitOfMeasure;

public class DeleteUnitOfMeasureCommand : IRequest<Result>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class DeleteUnitOfMeasureCommandHandler : IRequestHandler<DeleteUnitOfMeasureCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<DeleteUnitOfMeasureCommandHandler> logger;

    public DeleteUnitOfMeasureCommandHandler(AppDbContext dbContext, ILogger<DeleteUnitOfMeasureCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(DeleteUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.UnitOfMeasures
                .SingleOrDefaultAsync(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId), cancellationToken);

            if (entity == null)
            {
                return Result.NotFound();
            }

            dbContext.UnitOfMeasures.Remove(entity);

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

