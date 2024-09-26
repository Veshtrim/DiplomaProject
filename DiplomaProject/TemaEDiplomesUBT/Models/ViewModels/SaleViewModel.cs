namespace TemaEDiplomesUBT.Models.ViewModels
{
    public class SaleViewModel
    {
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? DocumentNumber { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public bool IsPaid { get; set; }
        public int? PaymentId { get; set; }
        public string? PaymentDocumentNumber { get; set; }

        public List<SaleDetailViewModel> SaleDetails { get; set; } = new List<SaleDetailViewModel>();
        public List<SaleDetailViewModel> OldSaleDetails { get; set; } = new List<SaleDetailViewModel>();
        public List<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>(); // To hold payment details

    }

    public class SaleDetailViewModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
