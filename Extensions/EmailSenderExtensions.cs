using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace InvoiceManager.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "PotwierdŸ adres e-mail", $"PotwierdŸ proszê swój adres e-mail <a href='{HtmlEncoder.Default.Encode(link)}'>klikaj¹c tutaj</a>.");
        }

        public static Task SendResetPasswordAsync(this IEmailSender emailSender, string email, string callbackUrl)
        {
            return emailSender.SendEmailAsync(email, "Resetowanie has³a", $"Zresetuj swoje has³o <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>klikaj¹c tutaj</a>.");
        }
    }
}
