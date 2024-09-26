using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Services.IServices
{
    public interface IWarehouseService
    {
        IEnumerable<WarehouseViewModel> GetAllWarehouses();
        WarehouseViewModel GetWarehouseById(int id);
        void CreateWarehouse(WarehouseViewModel model);
        void UpdateWarehouse(WarehouseViewModel model);
        void DeleteWarehouse(int id);
    }
}
