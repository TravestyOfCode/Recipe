using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;

namespace Recipe.Web.Application.Features.UnitOfMeasure;

public class CreateUnitOfMeasureCommand : IRequest<Result>
{
    public string Name { get; set; }

    public string Abbreviation { get; set; }

    public decimal? ConversionToGramsRatio { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class CreateUnitOfMeasureCommandHandler : IRequestHandler<CreateUnitOfMeasureCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<CreateUnitOfMeasureCommandHandler> logger;

    public CreateUnitOfMeasureCommandHandler(AppDbContext dbContext, ILogger<CreateUnitOfMeasureCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = dbContext.UnitOfMeasures.Add(new Data.UnitOfMeasure()
            {
                Abbreviation = request.Abbreviation,
                ConversionToGramsRatio = request.ConversionToGramsRatio,
                Name = request.Name,
                UserId = request.UserId
            });

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}", request);
            return Result.ServerError();
        }
    }
}

public class CreateUnitOfMeasureCommandValidator : AbstractValidator<CreateUnitOfMeasureCommand>
{
    public CreateUnitOfMeasureCommandValidator(AppDbContext dbContext)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(16);

        RuleFor(p => p.Abbreviation)
            .NotEmpty()
            .MaximumLength(8);

        RuleFor(p => p.ConversionToGramsRatio)
            .PrecisionScale(9, 5, true);

        RuleFor(p => p)
            .MustAsync(async (p, cancellationToken) =>
            {
                return !await dbContext.UnitOfMeasures.AnyAsync(u => u.Name.Equals(p.Name) && u.UserId.Equals(p.UserId), cancellationToken);
            }).WithMessage("There is already a Unit of Measure with this name.")
            .MustAsync(async (p, cancellationToken) =>
            {
                return !await dbContext.UnitOfMeasures.AnyAsync(u => u.Abbreviation.Equals(p.Abbreviation) && u.UserId.Equals(p.UserId), cancellationToken);
            }).WithMessage("There is already a Unit of Measure with this abbreviation.");
    }
}
