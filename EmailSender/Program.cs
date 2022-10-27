// See https://aka.ms/new-console-template for more information
using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using System.Net.Mail;
using System.Text;

Console.WriteLine("Hello, World!");

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

var email = await Email
    .From("mehmet.cinar@100comptable.com")
    .To("mehmet.cinar@100comptable.com", "Mehmet Cinar")
    .Subject("Thanks!")
    .UsingTemplate(template.ToString(), new { FirstName= "Mehmet", ProductName = "Laptop"})
    //.Body("Thanks for buying our product.")
    .SendAsync();


