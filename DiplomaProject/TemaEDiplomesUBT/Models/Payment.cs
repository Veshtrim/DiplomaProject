namespace TemaEDiplomesUBT.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int SaleId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string DocumentNumber { get; set; }
        public int? CustomerId { get; set; } 
        public bool IsPaid { get; set; }

        public Sale Sale { get; set; }
        public Customer Customer { get; set; }
    }
}
