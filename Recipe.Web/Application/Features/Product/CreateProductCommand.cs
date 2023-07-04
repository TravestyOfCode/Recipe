using Microsoft.AspNetCore.Mvc.ModelBinding;
using Recipe.Web.Data;
using System.ComponentModel.DataAnnotations;

namespace Recipe.Web.Application.Features.Product;

public class CreateProductCommand : IRequest<Result>
{
    [Required]
    [MaxLength(256)]
    public string Name { get; set; }

    [BindNever]
    public string UserId { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result>
{
    private readonly AppDbContext dbContext;

    private readonly ILogger<CreateProductCommandHandler> logger;

    public CreateProductCommandHandler(AppDbContext dbContext, ILogger<CreateProductCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = dbContext.Products.Add(new Data.Product()
            {
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

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(AppDbContext dbContext)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(p => p).MustAsync(async (p, cancellationToken) =>
        {
            return !await dbContext.Products.AnyAsync(u => u.Name.Equals(p.Name) && u.UserId.Equals(p.UserId), cancellationToken);
        }).WithMessage("There is already a Product with this name.");
    }
}
