namespace atmglobalapi.Services
{
    public interface IEmailService
    {
        Task<bool> SendStudentCredentialsAsync(
            string toEmail,
            string studentName,
            string loginId,
            string password,
            string srn
        );

        Task<bool> SendEmailAsync(
            string toEmail,
            string subject,
            string body
        );
    }
}