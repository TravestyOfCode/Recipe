using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Recipe.Web.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace Recipe.Web.Application.Features.Account;

public class RegisterAccountCommand : IRequest<Result>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Password and Confirm Password do not match.")]
    public string ConfirmPassword { get; set; }
}

public class RegisterAccountCommandHandler : IRequestHandler<RegisterAccountCommand, Result>
{
    private readonly UserManager<AppUser> userManager;

    private readonly LinkGenerator linkGenerator;

    private readonly ILogger<RegisterAccountCommandHandler> logger;

    private readonly IEmailSender emailSender;

    public RegisterAccountCommandHandler(UserManager<AppUser> userManager, ILogger<RegisterAccountCommandHandler> logger, LinkGenerator linkGenerator, IEmailSender emailSender)
    {
        this.userManager = userManager;
        this.logger = logger;
        this.linkGenerator = linkGenerator;
        this.emailSender = emailSender;
    }

    public async Task<Result> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
    {
        var user = new AppUser();

        await userManager.SetUserNameAsync(user, request.Email);

        await userManager.SetEmailAsync(user, request.Email);

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            logger.LogInformation("Account created with email {email}.", request.Email);

            var userId = await userManager.GetUserIdAsync(user);

            var confirmationCode = await userManager.GenerateEmailConfirmationTokenAsync(user);

            confirmationCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationCode));

            var callbackUrl = linkGenerator.GetPathByAction(
                action: "ConfirmEmail",
                controller: "Register",
                values: new { area = "Account", request.Email, userId, confirmationCode });

            logger.LogInformation("Confirmation Code: {confirmationCode}", confirmationCode);

            _ = emailSender.SendEmailAsync(request.Email, "Please confirm your email",
                $"Thank you for registering. Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Result.Ok();
        }

        var errors = result.Errors
            .Select(p => new Error(string.Empty, p.Description))
            .ToList();

        return Result.BadRequest(errors);
    }
}
