namespace TemaEDiplomesUBT.Models.ViewModels
{
    public class SupplierViewModel
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public List<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>();

        public string DisplayName => $"{Name} - {Email}";
    }
}
