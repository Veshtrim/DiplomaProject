using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace TemaEDiplomesUBT.Models
{
    public class Purchase
    {
        public int PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PurchaseDocumentNumber { get; set; }
        public bool IsPaid { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int? DebtId { get; set; }
        public string? DebtDocumentNumber { get; set; }
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
        public ICollection<Debt> Debts { get; set; } = new List<Debt>();
    }

    public class PurchaseDetail
    {
        public int PurchaseDetailId { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }

        public Purchase Purchase { get; set; }
        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }
    }

}
