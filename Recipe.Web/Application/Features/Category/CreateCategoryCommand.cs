using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.ComponentModel.DataAnnotations;

namespace Recipe.Web.Application.Features.Category;

public class CreateCategoryCommand : IRequest<Result>
{
    [Required]
    [MaxLength(32)]
    public string Name { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<CreateCategoryCommandHandler> logger;

    public CreateCategoryCommandHandler(AppDbContext dbContext, ILogger<CreateCategoryCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = dbContext.Categories.Add(new Data.Category()
            {
                Name = request.Name,
                UserId = request.UserId
            });

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

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator(AppDbContext dbContext)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(p => p).MustAsync(async (p, cancellationToken) =>
        {
            return !await dbContext.Categories.AnyAsync(c => c.Name.Equals(p.Name) && c.UserId.Equals(p.UserId), cancellationToken);
        }).WithMessage("Another Cateogry already exists with that name.");
    }
}
