using LegalPlatform.Application.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace LegalPlatform.Infrastructure.Utils;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOtpAsync(string toEmail, string code, CancellationToken cancellationToken)
    {
        var subject = "Your OTP Code";
        var body = $"Your OTP code is: {code}. It expires in 10 minutes.";
        await SendAsync(toEmail, subject, body, cancellationToken);
    }

    public async Task SendLawyerApprovalAsync(string toEmail, CancellationToken cancellationToken)
    {
        await SendAsync(toEmail, "Lawyer Profile Approved", "Your lawyer profile has been approved.", cancellationToken);
    }

    public async Task SendLawyerRejectionAsync(string toEmail, string reason, CancellationToken cancellationToken)
    {
        await SendAsync(toEmail, "Lawyer Profile Rejected", $"Your lawyer profile was rejected. Reason: {reason}", cancellationToken);
    }

    private async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var smtp = _configuration.GetSection("Smtp");
        var host = smtp["Host"]!;
        var port = int.Parse(smtp["Port"]!);
        var user = smtp["Username"]!;
        var pass = smtp["Password"]!;
        var from = smtp["From"]!;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Legal Platform", from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(user, pass, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}