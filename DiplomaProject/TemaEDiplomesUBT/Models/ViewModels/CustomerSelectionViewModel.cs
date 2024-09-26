using Microsoft.AspNetCore.Mvc.Rendering;

namespace TemaEDiplomesUBT.Models.ViewModels
{
    public class CustomerSelectionViewModel
    {
        public int SelectedCustomerId { get; set; }
        public IEnumerable<SelectListItem> Customers { get; set; }
        public IEnumerable<PaymentViewModel> UnpaidDocuments { get; set; }
    }

}
