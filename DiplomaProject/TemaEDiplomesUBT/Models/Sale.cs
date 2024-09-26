using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemaEDiplomesUBT.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? DocumentNumber { get; set; }
        public int CustomerId { get; set; } 
        public Customer Customer { get; set; }
        public bool IsPaid { get; set; }
        public int? PaymentId { get; set; } 
        public string? PaymentDocumentNumber { get; set; } 

        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }

    public class SaleDetail
    {
        public int SaleDetailId { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Sale Sale { get; set; }
        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }
    }

}
