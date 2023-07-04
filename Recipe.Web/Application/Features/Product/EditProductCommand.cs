using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.Linq;

namespace Recipe.Web.Application.Features.Product;

public class EditProductCommand : IRequest<Result>
{
    public int Id { get; set; }

    [BindNever]
    public string UserId { get; set; }

    public string Name { get; set; }
}

public class EditProductCommandHandler : IRequestHandler<EditProductCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<EditProductCommandHandler> logger;

    public EditProductCommandHandler(AppDbContext dbContext, ILogger<EditProductCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(EditProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Products
                .AsTracking()
                .Where(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId))
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return Result.NotFound();
            }

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

public class EditProductCommandValidator : AbstractValidator<EditProductCommand>
{
    public EditProductCommandValidator(AppDbContext dbContext)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(16);

        RuleFor(p => p)
            .MustAsync(async (p, cancellationToken) =>
            {
                return !await dbContext.Products.AnyAsync(u => u.Name.Equals(p.Name) && u.UserId.Equals(p.UserId), cancellationToken);
            }).WithMessage("There is already a Product with this name.");
    }
}