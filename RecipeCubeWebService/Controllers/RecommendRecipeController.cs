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
        public async Task<ActionResult<IEnumerable<RecommendRecipeDTO>>> RecommendRecipes([FromBody] RecommendRecipeRequestDTO request)
        {
            if (request == null || request.SelectedIngredients == null || request.SelectedIngredients.Count == 0)
            {
                return BadRequest(new { message = "食材 ID 列表不能為空" });
            }

            try
            {
                var userId = request.UserId;
                var selectedIngredients = request.SelectedIngredients;

                // 根據 userId 查找該用戶的庫存
                var userInventory = await _context.Inventories
                    .Where(inv => inv.UserId == userId)
                    .ToListAsync();

                var userInventoryIngredients = userInventory.Select(inv => inv.IngredientId).ToList();

                // 一次性查找所有食譜及其相關的成分信息
                var recipes = await _context.Recipes.ToListAsync();
                var recipeIds = recipes.Select(r => r.RecipeId).ToList();

                //var recipeIngredients = await _context.RecipeIngredients
                //    .Where(ri => recipeIds.Contains((int)ri.RecipeId))
                //    .ToListAsync();
                //var ingredients = await _context.Ingredients
                //    .Where(ing => recipeIngredients.Select(ri => ri.IngredientId).Contains(ing.IngredientId))
                //    .ToListAsync();
                //上面這段改用JOIN
                var recipeIngredients = await _context.RecipeIngredients
                    .Where(ri => recipeIds.Contains((int)ri.RecipeId))
                    .Join(_context.Ingredients,
                          recipeIngredient => recipeIngredient.IngredientId,
                          ingredient => ingredient.IngredientId,
                          (recipeIngredient, ingredient) => new
                          {
                              recipeIngredient.RecipeId,
                              recipeIngredient.IngredientId,
                              recipeIngredient.Quantity,
                              IngredientName = ingredient.IngredientName,
                              Unit = ingredient.Unit
                          })
                    .ToListAsync();

                var recommendedRecipes = new List<RecommendRecipeDTO>();

                foreach (var recipe in recipes)
                {
                    var currentRecipeIngredients = recipeIngredients
                        .Where(ri => ri.RecipeId == recipe.RecipeId)
                        .OrderBy(ri => ri.IngredientId)
                        .ToList();

                    var ingredientIds = currentRecipeIngredients.Select(ri => ri.IngredientId).ToList();
                    var ingredientNames = currentRecipeIngredients.Select(ri => ri.IngredientName).ToList();

                    var matchCount = selectedIngredients.Cast<int?>()
                        .Intersect(ingredientIds.Cast<int?>())
                        .Count();

                    if (matchCount > 0)
                    {
                        var matchRate = (double)matchCount / currentRecipeIngredients.Count;
                        var coverRate = (double)matchCount / selectedIngredients.Count;
                        var totalScore = matchRate * 0.5 + coverRate * 0.5;

                        // 檢查用戶是否擁有足夠的庫存來做這道食譜
                        bool isEnoughIngredients = true;
                        var missingIngredients = new List<MissingIngredientDTO>();

                        foreach (var recipeIngredient in currentRecipeIngredients)
                        {
                            var inventoryItem = userInventory.FirstOrDefault(inv => inv.IngredientId == recipeIngredient.IngredientId);
                            if (inventoryItem == null || inventoryItem.Quantity < recipeIngredient.Quantity)
                            {
                                isEnoughIngredients = false;
                                var missingQuantity = recipeIngredient.Quantity - (inventoryItem?.Quantity ?? 0);

                                missingIngredients.Add(new MissingIngredientDTO
                                {
                                    IngredientId = (int)recipeIngredient.IngredientId,
                                    IngredientName = recipeIngredient.IngredientName,
                                    MissingQuantity = missingQuantity,
                                    Unit = recipeIngredient.Unit
                                });
                            }
                        }

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
                            IngredientNames = ingredientNames,
                            IsEnoughIngredients = isEnoughIngredients,
                            MissingIngredients = missingIngredients
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

                if (!sortedRecipes.Any())
                {
                    return NotFound(new { message = "未找到符合條件的食譜" });
                }

                return Ok(sortedRecipes);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = "發生伺服器錯誤", error = ex.Message });
            }
        }
    }
}