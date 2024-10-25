namespace RecipeCubeWebService.DTO
{
    public class MissingIngredientDTO
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; }
        public decimal? MissingQuantity { get; set; }
        public string Unit { get; set; }
    }
}
