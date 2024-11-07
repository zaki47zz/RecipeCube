using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCubeWebService.Models;
using RecipeCubeWebService.DTO;


namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public HomeController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: api/<HomeController>
        [HttpGet]
        public async Task<ActionResult<homeDTO>> GetInfo()
        {
            int recipeAmount = await _context.Recipes.CountAsync();
            int ingredientAmount = await _context.Ingredients.CountAsync();
            int GroupAmount = await _context.UserGroups.CountAsync();
            int UserAmount = await _context.Users.CountAsync();
            homeDTO homeDTO = new homeDTO{ 
                RecipeAmount = recipeAmount,
                IngredientAmount = ingredientAmount,
                GroupAmount = GroupAmount,
                UserAmount = UserAmount
            };
            return homeDTO;
        }

        // GET: api/Home/Recommend
        [HttpGet("Recommend")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecommendRecipe()
        {
            var recipes = await _context.Recipes
                .Where(recipe => recipe.UserId == "0" && recipe.Status == true)
                .ToListAsync();

            int recipeAmount = recipes.Count;
            List<Recipe> selectedRecipes = new List<Recipe>();
            Random random = new Random();
            HashSet<int> selectedIndices = new HashSet<int>();

            // 隨機選取5個食譜
            while (selectedIndices.Count < 5 && selectedIndices.Count < recipeAmount)
            {
                int randomIndex = random.Next(0, recipeAmount);
                selectedIndices.Add(randomIndex);
            }

            // 根據隨機索引選擇食譜
            foreach (int index in selectedIndices)
            {
                selectedRecipes.Add(recipes[index]);
            }

            return Ok(selectedRecipes);
        }
    }
}

