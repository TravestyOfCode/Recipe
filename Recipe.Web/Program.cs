using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recipe.Web.Data;

namespace Recipe.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add and configure DBContext
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        // Add and configure Identity
        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;

            options.Password.RequiredLength = 8;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

        // Add and configure Controllers
        builder.Services.AddControllersWithViews();

        // Add and configure Validators
        builder.Services.AddValidatorsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

        // Add and configure MediatR
        builder.Services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

            options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Add Emails sender
        // TODO: Add a real email sender
        builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapAreaControllerRoute(
            name: "Account",
            areaName: "account",
            pattern: "account/{controller=Manage}/{action=Index}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
