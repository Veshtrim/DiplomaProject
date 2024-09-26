namespace TemaEDiplomesUBT.Models.ViewModels
{
    public class StockTransferViewModel
    {
        public int SourceWarehouseId { get; set; }
        public int DestinationWarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
