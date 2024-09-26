using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Workflow.WorkflowData
{
    public class SaleWorkflowData
    {
        public string DocumentNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleDetailViewModel> SaleDetails { get; set; }
        public string ErrorMessage { get; set; }
        public string CustomerName { get; set; }
        public List<SaleDetailViewModel> OldSaleDetails { get; set; }
        public List<SaleDetailViewModel> NewSaleDetails { get; set; }
    }
}
