using System.Collections.Generic;
using System.Threading.Tasks;
using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Services.IServices
{
    public interface IDebtService
    {
        Task<List<DebtViewModel>> GetUnpaidDebtsBySupplierAsync(int supplierId);
        Task MarkDebtAsPaidAsync(int debtId);
        Task AddDebtAsync(DebtViewModel model);
        Task UpdateDebtAsync(DebtViewModel model);
        Task DeleteDebtAsync(int debtId);
    }
}
