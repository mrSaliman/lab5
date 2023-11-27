using lab5.Areas.FurnitureFactory.Models;

namespace lab5.Areas.FurnitureFactory.ViewModels
{
    public class CustomerViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public string CompanyName { get; set; } = null!;

        public string RepresentativeLastName { get; set; } = null!;

        public string RepresentativeFirstName { get; set; } = null!;

        public string RepresentativeMiddleName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Address { get; set; } = null!;
    }
}
