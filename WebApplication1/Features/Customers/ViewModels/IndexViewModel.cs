using WebApplication1.Models;

namespace WebApplication1.Features.Customers.ViewModels
{
    public class IndexViewModel
    {
        public PaginatedResult<CustomerDto> Result { get; set; }
    }
}