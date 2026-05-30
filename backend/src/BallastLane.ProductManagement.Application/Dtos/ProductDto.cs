using BallastLane.ProductManagement.Domain.Entities;

namespace BallastLane.ProductManagement.Application.Dtos
{
    /// <summary>
    /// Product DTO.
    /// </summary>
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ProductDto() { }

        public ProductDto(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
            Stock = product.Stock;
            CreatedAt = product.CreatedAt;
            UpdatedAt = product.UpdatedAt;
        }
    }
}
