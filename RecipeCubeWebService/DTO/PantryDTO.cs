namespace RecipeCubeWebService.DTO
{
    public class PantryDTO
    {
        public int PantryId { get; set; }

        public int? GroupId { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int? IngredientId { get; set; }

        public string? IngredientName { get; set; }

        public decimal? Quantity { get; set; }

        public string? Unit { get; set; }

        public string Action { get; set; }

        public DateTime? Time { get; set; }
    }
}
