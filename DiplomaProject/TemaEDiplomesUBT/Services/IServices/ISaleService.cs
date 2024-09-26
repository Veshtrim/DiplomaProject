using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Services.IServices
{
    public interface ISaleService
    {
        Task<List<Sale>> GetAllSalesAsync();
        Task<SaleViewModel> GetSaleByIdAsync(int saleId);
        Task<WorkflowResult> CreateSaleAsync(SaleViewModel model);
        Task UpdateSaleAsync(SaleViewModel saleViewModel);
        Task DeleteSaleAsync(int id);
    }
}
