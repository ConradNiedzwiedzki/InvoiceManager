using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceManager.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        public string OwnerId { get; set; }

        [Display(Name = "ID księgowego")]
        public string AccountantId { get; set; }

        [Required(ErrorMessage = "Wpisz nazwę firmy do faktury")]
        [Display(Name = "Nazwa firmy do faktury")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Wpisz datę wystawienia faktury")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data wystawienia")]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "Wpisz kwotę (np. 12.34)")]
        [Display(Name = "Kwota")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Wpisz datę opłacenia faktury")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Termin opłacenia")]
        public DateTime DueDate { get; set; }

        public InvoiceStatus Status { get; set; }

        [Display(Name = "Nazwa firmy klienta")]
        public string UserCompanyName { get; set; }
    }

    public enum InvoiceStatus
    {
        [Display(Name = "Oczekuje")] Submitted,
        [Display(Name = "Wystawiono")] Approved
    }
}