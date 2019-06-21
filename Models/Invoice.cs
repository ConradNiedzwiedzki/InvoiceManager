using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceManager.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        public string OwnerId { get; set; }

        [Display(Name = "Nazwa firmy")]
        public string CompanyName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data wystawienia")]
        public DateTime IssueDate { get; set; }

        [Display(Name = "Kwota")]
        public double Amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Termin opłacenia")]
        public DateTime DueDate { get; set; }

        public InvoiceStatus Status { get; set; }
    }

    public enum InvoiceStatus
    {
        [Display(Name = "Oczekuje")] Submitted,
        [Display(Name = "Wystawiono")] Approved
    }
}