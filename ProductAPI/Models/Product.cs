using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage= "The name is required")]
        [StringLength(100,ErrorMessage = "The name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Range(minimum: 0.01, double.MaxValue, ErrorMessage = "The price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }
    }
}
