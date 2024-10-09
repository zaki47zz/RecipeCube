namespace RecipeCubeWebService.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int? IngredientId { get; set; }

        public int? Price { get; set; }

        public int? Stock { get; set; }

        public bool? Status { get; set; }

        public string Photo { get; set; }

        public string? Category { get; set; }

        public string? unit { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? UnitQuantity { get; set; }
    }
}
