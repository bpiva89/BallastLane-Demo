namespace BallastLane.ProductManagement.Domain.Entities
{
    /// <summary>
    /// Product entity.
    /// </summary>
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
