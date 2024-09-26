using System.Collections.Generic;
using System.Threading.Tasks;
using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Services.IServices
{
    public interface IPaymentService
    {
        Task<List<PaymentViewModel>> GetUnpaidPaymentsByCustomerAsync(int customerId);
        Task AddPaymentAsync(PaymentViewModel model);
        Task MarkPaymentAsPaidAsync(int paymentId);

    }
}
