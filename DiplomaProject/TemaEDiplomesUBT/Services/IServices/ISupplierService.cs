using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Services.IServices
{
    public interface ISupplierService
    {
        Task<List<SupplierViewModel>> GetAllSupplierAsync();
        Task<SupplierViewModel> GetSupplierByIdAsync(int id);
        Task CreateSupplierAsync(SupplierViewModel supplier);
        Task UpdateSupplierAsync(SupplierViewModel supplier);
        Task DeleteSupplierAsync(int id);
    }
}
