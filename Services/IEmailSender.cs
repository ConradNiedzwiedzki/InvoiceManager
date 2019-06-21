using System.Threading.Tasks;

namespace InvoiceManager.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
