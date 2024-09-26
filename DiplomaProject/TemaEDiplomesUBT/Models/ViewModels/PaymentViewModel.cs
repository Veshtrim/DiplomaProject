using Microsoft.AspNetCore.Mvc.Rendering;

namespace TemaEDiplomesUBT.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }
        public int SaleId { get; set; }
        public int CustomerId { get; set; }
        public string DocumentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string? CustomerName { get; set; }
        public bool IsPaid { get; set; }
        public bool IsSelectedForPayment { get; set; }  
        public int? SelectedCustomerId { get; set; }
        public SelectList Customers { get; set; } = new SelectList(new List<SelectListItem>());
    }
}
