using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Workflow.WorkflowData
{
    public class PurchaseWorkflowData
    {
        public string PurchaseDocumentNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseDetailViewModel> PurchaseDetails { get; set; }
        public string ErrorMessage { get; set; }
        public string SupplierName { get; set; }
        public List<PurchaseDetailViewModel> OldPurchase { get; set; }
        public List<PurchaseDetailViewModel> NewPurchase { get; set; }
    }
}
