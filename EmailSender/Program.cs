// See https://aka.ms/new-console-template for more information
using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Net.Mail;
using System.Text;

Console.WriteLine("--- Email Sender ---");

var builder = new ConfigurationBuilder();
BuildConfig(builder);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger.Information("Application Starting");

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<IGreetingService, GreetingService>();
    })
    .UseSerilog()
    .Build();

var svc = ActivatorUtilities.CreateInstance<GreetingService>(host.Services);
svc.Run();

var sender = new SmtpSender(() => new System.Net.Mail.SmtpClient("localhost")
{
    EnableSsl = false,
    //DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
    //PickupDirectoryLocation = @"C:\Sources\Emails"

    DeliveryMethod = SmtpDeliveryMethod.Network,
    Port = 25
});

StringBuilder template = new();
template.AppendLine("Dear @Model.FirstName,");
template.AppendLine("<p>Thanks for purchasing @Model.ProductName. We hope you enjoy it.</p>");
template.AppendLine("- Mcinar Team");

Email.DefaultSender = sender;
Email.DefaultRenderer = new RazorRenderer();

//var email = await Email
//    .From("mehmet.cinar@100comptable.com")
//    .To("mehmet.cinar@100comptable.com", "Mehmet Cinar")
//    .Subject("Thanks!")
//    .UsingTemplate(template.ToString(), new { FirstName= "Mehmet", ProductName = "Laptop"})
//    //.Body("Thanks for buying our product.")
//    .SendAsync();



static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables();
}
