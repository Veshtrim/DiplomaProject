using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Services.IServices
{
    public interface IPurchaseService
    {
        Task<List<Purchase>> GetAllPurchasesAsync();
        Task<PurchaseViewModel> GetPurchaseByIdAsync(int id);
        Task<WorkflowResult> CreatePurchaseAsync(PurchaseViewModel model);
        Task UpdatePurchaseAsync(PurchaseViewModel model);
        Task DeletePurchaseAsync(int id);
    }

}
