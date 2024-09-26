namespace TemaEDiplomesUBT.Models.ViewModels
{
    public class PurchaseViewModel
    {
        public int PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PurchaseDocumentNumber { get; set; } 
        public bool IsPaid { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int? DebtId { get; set; }
        public string? DebtDocumentNumber { get; set; }
        public List<PurchaseDetailViewModel> PurchaseDetails { get; set; } = new List<PurchaseDetailViewModel>();
    }

    public class PurchaseDetailViewModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }

        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
    }

}
