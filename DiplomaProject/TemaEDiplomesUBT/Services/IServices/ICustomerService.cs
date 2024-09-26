using System.Collections.Generic;
using System.Threading.Tasks;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Services.IServices
{
    public interface ICustomerService
    {
        Task<List<CustomerViewModel>> GetAllCustomersAsync();
        Task<CustomerViewModel> GetCustomerByIdAsync(int customerId);
        Task AddCustomerAsync(CustomerViewModel model);
        Task UpdateCustomerAsync(CustomerViewModel model);
        Task DeleteCustomerAsync(int customerId);
        Task<List<Sale>> GetUnpaidDocumentsByCustomerAsync(int customerId);
    }
}
