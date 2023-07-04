using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.ComponentModel.DataAnnotations;

namespace Recipe.Web.Application.Features.Category;

public class EditCategoryCommand : IRequest<Result>
{
    public int Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string Name { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class EditCategoryCommandHandler : IRequestHandler<EditCategoryCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<EditCategoryCommandHandler> logger;

    public EditCategoryCommandHandler(AppDbContext dbContext, ILogger<EditCategoryCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Categories
                .AsTracking()
                .SingleOrDefaultAsync(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId), cancellationToken);

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

public class EditCategoryCommandValidator : AbstractValidator<EditCategoryCommand>
{
    public EditCategoryCommandValidator(AppDbContext dbContext)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(p => p).MustAsync(async (p, cancellationToken) =>
        {
            return !await dbContext.Categories.AnyAsync(c => c.Name.Equals(p.Name) && c.UserId.Equals(p.UserId) && c.Id != p.Id);
        }).WithMessage("Another Category already exists with this name.");
    }
}
