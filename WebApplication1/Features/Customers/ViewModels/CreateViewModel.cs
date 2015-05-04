using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Features.Customers.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}