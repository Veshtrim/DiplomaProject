using System.ComponentModel.DataAnnotations;

namespace TemaEDiplomesUBT.Models.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number.")]
        public string Phone { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters.")]
        public string Address { get; set; }

      

        public List<SaleViewModel> Sales { get; set; } = new List<SaleViewModel>();
        public List<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>();

        public string DisplayName => $"{Name} - {Email}";
    }
}
