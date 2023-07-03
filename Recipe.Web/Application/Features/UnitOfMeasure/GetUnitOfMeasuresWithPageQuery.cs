using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.UnitOfMeasure;

public class GetUnitOfMeasuresWithPageQuery : PageQuery, IRequest<Result<UnitOfMeasureListModel>>
{
    [BindNever]
    public string UserId { get; set; }
}

public class GetUnitOfMeasuresWithPageQueryHandler : IRequestHandler<GetUnitOfMeasuresWithPageQuery, Result<UnitOfMeasureListModel>>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<GetUnitOfMeasuresWithPageQueryHandler> logger;

    public GetUnitOfMeasuresWithPageQueryHandler(AppDbContext dbContext, ILogger<GetUnitOfMeasuresWithPageQueryHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result<UnitOfMeasureListModel>> Handle(GetUnitOfMeasuresWithPageQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await dbContext.UnitOfMeasures.Where(p => p.UserId.Equals(request.UserId))
                .AsPageQuery(request)
                .Select(p => new UnitOfMeasureModel()
                {
                    Abbreviation = p.Abbreviation,
                    ConversionToGramsRatio = p.ConversionToGramsRatio,
                    Id = p.Id,
                    Name = p.Name
                }).ToListAsync(cancellationToken);

            double totalCount = await dbContext.UnitOfMeasures.Where(p => p.UserId.Equals(request.UserId))
                .CountAsync(cancellationToken);

            return Result.Ok(new UnitOfMeasureListModel(entities, request, totalCount));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError<UnitOfMeasureListModel>();
        }
    }
}