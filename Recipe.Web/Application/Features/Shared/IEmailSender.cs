namespace Recipe.Web.Application.Features.Shared;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}

public class NoOpEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}