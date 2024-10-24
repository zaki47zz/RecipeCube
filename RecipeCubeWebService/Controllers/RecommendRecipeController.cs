using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCubeWebService.DTO;
using RecipeCubeWebService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendRecipeController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public RecommendRecipeController(RecipeCubeContext context)
        {
            _context = context;
        }

        [HttpPost("Recommend")]
        public async Task<ActionResult<IEnumerable<RecommendRecipeDTO>>> RecommendRecipes([FromBody] List<int> selectedIngredients)
        {
            if (selectedIngredients == null || selectedIngredients.Count == 0)
            {
                return BadRequest(new { message = "食材 ID 列表不能為空" });
            }

            var recipes = await _context.Recipes.ToListAsync();
            var recommendedRecipes = new List<RecommendRecipeDTO>();


            foreach (var recipe in recipes)
            {
                var recipeIngredients = await _context.RecipeIngredients
                    .Where(ri => ri.RecipeId == recipe.RecipeId)
                    .Select(ri => ri.IngredientId)
                    .ToListAsync();
                var ingredientIds = await _context.RecipeIngredients
                    .Where(ri => ri.RecipeId == recipe.RecipeId)
                    .Select(ri => ri.IngredientId)
                    .ToListAsync();

                var ingredientNames = await _context.Ingredients
                    .Where(ing => ingredientIds.Contains(ing.IngredientId))
                    .Select(ing => ing.IngredientName)
                    .ToListAsync();
                var matchCount = selectedIngredients.Cast<int?>().Intersect(recipeIngredients.Cast<int?>()).Count();

                if (matchCount > 0)
                {
                    var matchRate = (double)matchCount / recipeIngredients.Count;
                    var coverRate = (double)matchCount / selectedIngredients.Count;
                    var totalScore = matchRate * 0.5 + coverRate * 0.5;

                    recommendedRecipes.Add(new RecommendRecipeDTO
                    {
                        RecipeId = recipe.RecipeId,
                        RecipeName = recipe.RecipeName,
                        MatchRate = matchRate,
                        CoverRate = coverRate,
                        TotalScore = totalScore,
                        photoName = recipe.Photo,
                        Restriction = recipe.Restriction,
                        WestEast = recipe.WestEast,
                        Category = recipe.Category,
                        DetailedCategory = recipe.DetailedCategory,
                        IngredientIds = ingredientIds,
                        IngredientNames = ingredientNames

                    });
                }
            }

            var sortedRecipes = recommendedRecipes
                .Select(r => {
                    r.MatchRate = Math.Round(r.MatchRate, 3);
                    r.CoverRate = Math.Round(r.CoverRate, 3);
                    r.TotalScore = Math.Round(r.TotalScore, 3);
                    return r;
                })
                .OrderByDescending(r => r.TotalScore)
                .ToList();
            if (sortedRecipes == null || !sortedRecipes.Any())
            {
                return NotFound(new { message = "未找到符合條件的食譜" });
            }
            return Ok(sortedRecipes);
        }
    }
}
