using System.Net;
using System.Net.Mail;

namespace atmglobalapi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendStudentCredentialsAsync(
            string toEmail,
            string studentName,
            string loginId,
            string password,
            string srn)
        {
            try
            {
                var subject = "Your Student Account Credentials - U77 University";

                var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4; }}
        .content {{ background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .credentials {{ background-color: #f8f9fa; padding: 15px; border-left: 4px solid #007bff; margin: 20px 0; }}
        .credentials p {{ margin: 10px 0; }}
        .credentials strong {{ color: #007bff; }}
        .footer {{ text-align: center; margin-top: 20px; padding-top: 20px; border-top: 1px solid #ddd; color: #666; font-size: 12px; }}
        .warning {{ background-color: #fff3cd; padding: 10px; border-left: 4px solid #ffc107; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='content'>
            <div class='header'>
                <h1>Welcome to U77 University!</h1>
            </div>
            
            <h2>Dear {studentName},</h2>
            
            <p>Congratulations! Your student account has been successfully created.</p>
            
            <div class='credentials'>
                <h3>Your Login Credentials:</h3>
                <p><strong>Student Registration Number (SRN):</strong> {srn}</p>
                <p><strong>Login ID:</strong> {loginId}</p>
                <p><strong>Temporary Password:</strong> {password}</p>
                <p><strong>Portal URL:</strong> <a href='{_configuration["serverurl"]}'>{_configuration["serverurl"]}</a></p>
            </div>
            
            <div class='warning'>
                <p><strong>⚠️ Important:</strong></p>
                <ul>
                    <li>Please change your password after your first login</li>
                    <li>Do not share your credentials with anyone</li>
                    <li>Keep this email secure for your records</li>
                </ul>
            </div>
            
            <p>If you have any questions or need assistance, please contact our support team.</p>
            
            <p>Best regards,<br>
            <strong>U77 University Administration</strong></p>
            
            <div class='footer'>
                <p>This is an automated email. Please do not reply to this message.</p>
                <p>&copy; 2026 U77 University. All rights reserved.</p>
            </div>
        </div>
    </div>
</body>
</html>";

                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending student credentials: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var senderName = _configuration["EmailSettings:SenderName"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = enableSsl;
                    client.Credentials = new NetworkCredential(username, password);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail ?? "", senderName ?? "U77 University"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}