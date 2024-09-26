namespace TemaEDiplomesUBT.Workflow.WorkflowData
{
    public class DebtPaymentWorkflowData
    {
        public List<int> PurchaseIds { get; set; }
        public int PurchaseId { get; set; }
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDocumentNumber { get; set; }
    }
}
