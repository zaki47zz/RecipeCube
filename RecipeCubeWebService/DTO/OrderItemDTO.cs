namespace RecipeCubeWebService.DTO;

    public class OrderItemDTO
    {
        public long? OrderId { get; set; }
        public int? ProductId { get; set; }

        public int? Quantity { get; set; }

        public int? Price { get; set; }
    }

