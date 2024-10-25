namespace RecipeCubeWebService.DTO
{
    public class IngredientDTO
    {
        public int IngredientId { get; set; }

        public string IngredientName { get; set; }

        public string Category { get; set; }

        public string Synonym { get; set; }

        public int? ExpireDay { get; set; } = 7;

        public string Unit { get; set; } = "克";

        public decimal? Gram { get; set; }

        public IFormFile Photo { get; set; }
    }
}
