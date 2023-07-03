using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.UnitOfMeasure;

public class GetUnitOfMeasureByIdQuery : IRequest<Result<UnitOfMeasureModel>>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class GetUnitOfMeasureByIdQueryHandler : IRequestHandler<GetUnitOfMeasureByIdQuery, Result<UnitOfMeasureModel>>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<GetUnitOfMeasureByIdQueryHandler> logger;

    public GetUnitOfMeasureByIdQueryHandler(AppDbContext dbContext, ILogger<GetUnitOfMeasureByIdQueryHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result<UnitOfMeasureModel>> Handle(GetUnitOfMeasureByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.UnitOfMeasures
                .Where(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId))
                .Select(p => new UnitOfMeasureModel()
                {
                    Abbreviation = p.Abbreviation,
                    ConversionToGramsRatio = p.ConversionToGramsRatio,
                    Id = p.Id,
                    Name = p.Name
                }).SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return Result.NotFound<UnitOfMeasureModel>();
            }

            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError<UnitOfMeasureModel>();
        }
    }
}
