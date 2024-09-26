using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<Debt> Debts { get; set; } = new List<Debt>();
    }
}
