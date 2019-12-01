using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace InvoiceManager.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, Resources.ApplicationTexts.ConfirmEmailAdress, string.Format(Resources.ApplicationTexts.ConfirmEmailAdressByClickingHere, HtmlEncoder.Default.Encode(link)));
        }

        public static Task SendResetPasswordAsync(this IEmailSender emailSender, string email, string callbackUrl)
        {
            return emailSender.SendEmailAsync(email, Resources.ApplicationTexts.PasswordReset, string.Format(Resources.ApplicationTexts.ResetYourPasswordByClickingHere, HtmlEncoder.Default.Encode(callbackUrl)));
        }
    }
}
