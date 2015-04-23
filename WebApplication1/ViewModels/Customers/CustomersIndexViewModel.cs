using WebApplication1.Models;

namespace WebApplication1.ViewModels.Customers
{
    public class CustomersIndexViewModel
    {
        public PaginatedResult<CustomerDto> Result { get; set; }
    }

    public class CustomersCreateViewModel
    {
        public string Name { get; set; }
    }

    public class CustomersEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}