namespace TemaEDiplomesUBT.Models
{
    public class Debt
    {
        public int DebtId { get; set; }
        public int PurchaseId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string DocumentNumber { get; set; }
        public int? SupplierId { get; set; }
        public bool IsPaid { get; set; }

        public Purchase Purchase { get; set; }
        public Supplier Supplier { get; set; }
    }
}
