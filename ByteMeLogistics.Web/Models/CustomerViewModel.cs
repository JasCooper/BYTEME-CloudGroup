using System.ComponentModel.DataAnnotations;

namespace ByteMeLogistics.Web.Models
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
    }
}