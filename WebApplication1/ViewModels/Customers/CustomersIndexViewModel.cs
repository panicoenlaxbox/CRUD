using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels.Customers
{
    public class CustomersIndexViewModel
    {
        public PaginatedResult<CustomerDto> Result { get; set; }
    }

    public class CustomersCreateViewModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class CustomersEditViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}