using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.UnitOfMeasure;

public class GetUnitOfMeasuresAsDictionaryQuery : IRequest<Result<Dictionary<int, string>>>
{
    [BindNever]
    public string UserId { get; set; }

    public GetUnitOfMeasuresAsDictionaryQuery() { }

    public GetUnitOfMeasuresAsDictionaryQuery(string userId) => UserId = userId;
}

public class GetUnitOfMeasuresAsDictionaryQueryHandler : IRequestHandler<GetUnitOfMeasuresAsDictionaryQuery, Result<Dictionary<int, string>>>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<GetUnitOfMeasuresAsDictionaryQueryHandler> logger;

    public GetUnitOfMeasuresAsDictionaryQueryHandler(AppDbContext dbContext, ILogger<GetUnitOfMeasuresAsDictionaryQueryHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result<Dictionary<int, string>>> Handle(GetUnitOfMeasuresAsDictionaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return Result.Ok(await dbContext.UnitOfMeasures
                .Where(p => p.UserId.Equals(request.UserId))
                .ToDictionaryAsync(p => p.Id, p => p.Name, cancellationToken));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError<Dictionary<int, string>>();
        }
    }
}
