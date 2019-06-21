using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace InvoiceManager.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Potwierd� adres e-mail", $"Potwierd� prosz� sw�j adres e-mail <a href='{HtmlEncoder.Default.Encode(link)}'>klikaj�c tutaj</a>.");
        }

        public static Task SendResetPasswordAsync(this IEmailSender emailSender, string email, string callbackUrl)
        {
            return emailSender.SendEmailAsync(email, "Resetowanie has�a", $"Zresetuj swoje has�o <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>klikaj�c tutaj</a>.");
        }
    }
}
