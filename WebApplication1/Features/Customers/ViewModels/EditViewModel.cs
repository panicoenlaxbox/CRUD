using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Features.Customers.ViewModels
{
    public class EditViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}