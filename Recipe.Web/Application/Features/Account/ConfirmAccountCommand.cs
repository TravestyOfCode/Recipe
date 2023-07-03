using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Recipe.Web.Data;
using System.Text;

namespace Recipe.Web.Application.Features.Account;

public class ConfirmAccountCommand : IRequest<Result>
{
    public string Email { get; set; }

    public string UserId { get; set; }

    public string ConfirmationCode { get; set; }
}

public class ConfirmAccountCommandHandler : IRequestHandler<ConfirmAccountCommand, Result>
{
    private readonly UserManager<AppUser> userManager;

    public ConfirmAccountCommandHandler(UserManager<AppUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<Result> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == null || request.ConfirmationCode == null)
        {
            return Result.BadRequest(new Error(string.Empty, "We were unable to confirm the account. Please check your email and try again."));
        }

        var user = await userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return Result.BadRequest(new Error(string.Empty, "We were unable to confirm the account. Please check your email and try again."));
        }

        request.ConfirmationCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ConfirmationCode));

        var result = await userManager.ConfirmEmailAsync(user, request.ConfirmationCode);

        return result.Succeeded ? Result.Ok() : Result.BadRequest(new Error(string.Empty, "We were unable to confirm the account. Please check your email and try again."));
    }
}
