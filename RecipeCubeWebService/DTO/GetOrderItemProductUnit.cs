namespace RecipeCubeWebService.DTO;

    public class GetOrderItemProductUnit
    {
        public long? OrderId { get; set; }
        public int? ProductId { get; set; }

        public int? Quantity { get; set; }

        public int? Price { get; set; }

        public string ProductName { get; set; }

        public int? IngredientId { get; set; }

        public string Photo { get; set; }

        public string? Unit { get; set; }

        public decimal? UnitQuantity { get; set; }
    }

