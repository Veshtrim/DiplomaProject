namespace TemaEDiplomesUBT.Workflow.WorkflowData
{
    public class PaymentWorkflowData
    {
        public List<int> SaleIds { get; set; }
        public int SaleId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDocumentNumber { get; set; }
    }
}
