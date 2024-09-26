namespace TemaEDiplomesUBT.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public ICollection<Sale> Sales { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}

