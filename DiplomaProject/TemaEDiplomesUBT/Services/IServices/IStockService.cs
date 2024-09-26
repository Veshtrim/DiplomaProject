using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Services.IServices
{
    public interface IStockService
    {
        IEnumerable<StockViewModel> GetStocks();
        Task AdjustStock(int productId, int warehouseId, int quantity);
        Task TransferStock(int sourceWarehouseId, int destinationWarehouseId, int productId, int quantity);
    }
}
