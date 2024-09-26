using Microsoft.AspNetCore.Mvc.Rendering;

namespace TemaEDiplomesUBT.Models.ViewModels
{
    public class DebtViewModel
    {
        public int DebtId { get; set; }
        public int PurchaseId { get; set; }
        public int SupplierId { get; set; }
        public string DocumentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string? SupplierName { get; set; }
        public bool IsPaid { get; set; }
        public bool IsSelectedForPayment { get; set; }
        public int? SelectedSupplierId { get; set; }
        public SelectList Suppliers { get; set; } = new SelectList(new List<SelectListItem>());
    }
}
