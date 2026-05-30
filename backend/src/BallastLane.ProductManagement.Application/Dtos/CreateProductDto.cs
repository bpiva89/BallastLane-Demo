using System.ComponentModel.DataAnnotations;

namespace BallastLane.ProductManagement.Application.Dtos
{
    /// <summary>
    /// DTO for creating a new product.
    /// </summary>
    public class CreateProductDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = default!;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
