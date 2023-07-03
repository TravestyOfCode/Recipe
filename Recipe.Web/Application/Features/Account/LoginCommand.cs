using Microsoft.AspNetCore.Identity;
using Recipe.Web.Data;
using System.ComponentModel.DataAnnotations;

namespace Recipe.Web.Application.Features.Account;

public class LoginCommand : IRequest<Result>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result>
{
    private readonly SignInManager<AppUser> signInManager;

    private readonly ILogger<LoginCommandHandler> logger;

    public LoginCommandHandler(SignInManager<AppUser> signInManager, ILogger<LoginCommandHandler> logger)
    {
        this.signInManager = signInManager;
        this.logger = logger;
    }

    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                logger.LogInformation("User logged in with {email}", request.Email);

                return Result.Ok();
            }

            return Result.BadRequest(new Error(string.Empty, "Invalid UserName or Password."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling {request}.", request);

            return Result.ServerError();
        }
    }
}
