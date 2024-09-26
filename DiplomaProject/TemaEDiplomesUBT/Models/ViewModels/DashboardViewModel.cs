using System;

namespace TemaEDiplomesUBT.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalSales { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalDebts { get; set; }
        public int TotalPayments { get; set; }
        public decimal TotalAmountPaidOnSales { get; set; }
        public decimal TotalAmountPaidOnPurchases { get; set; }
        public IEnumerable<WarehouseViewModel> Warehouses { get; set; }
        public IEnumerable<StockViewModel> StockItems { get; set; }
    }
}
