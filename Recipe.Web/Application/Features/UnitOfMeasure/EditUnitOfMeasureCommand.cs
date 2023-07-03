using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.UnitOfMeasure;

public class EditUnitOfMeasureCommand : IRequest<Result>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }

    public string Name { get; set; }

    public string Abbreviation { get; set; }

    public decimal? ConversionToGramsRatio { get; set; }
}

public class EditUnitOfMeasureCommandHandler : IRequestHandler<EditUnitOfMeasureCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<EditUnitOfMeasureCommandHandler> logger;

    public EditUnitOfMeasureCommandHandler(AppDbContext dbContext, ILogger<EditUnitOfMeasureCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(EditUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.UnitOfMeasures
                .AsTracking()
                .Where(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId))
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return Result.NotFound();
            }

            entity.Abbreviation = request.Abbreviation;
            entity.ConversionToGramsRatio = request.ConversionToGramsRatio;
            entity.Name = request.Name;

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

public class EditUnitOfMeasureCommandValidator : AbstractValidator<EditUnitOfMeasureCommand>
{
    public EditUnitOfMeasureCommandValidator(AppDbContext dbContext)
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