using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeCubeWebService.DTO;
using RecipeCubeWebService.Models;
using Newtonsoft.Json;

namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public RecipesController(RecipeCubeContext context)
        {
            _context = context;
        }

        //// GET: api/Recipes
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        //{
        //    return await _context.Recipes.ToListAsync();
        //}
        // GET: api/Recipes
        // GET: api/Recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipes()
        {
            var recipes = await _context.Recipes
                .Where(r => r.Status == true) // 只取狀態為 true 的食譜
                .ToListAsync();

            var recipeDtos = new List<RecipeDto>();
            foreach (var recipe in recipes)
            {
                // 手動查詢與該 Recipe 關聯的 RecipeIngredients
                var recipeIngredients = await _context.RecipeIngredients
                    .Where(ri => ri.RecipeId == recipe.RecipeId)
                    .ToListAsync();

                // 查詢對應的食材
                var ingredientIds = recipeIngredients.Select(ri => ri.IngredientId).Distinct().ToList();
                var ingredients = await _context.Ingredients
                    .Where(i => ingredientIds.Contains(i.IngredientId))
                    .ToListAsync();

                // 查詢同義字
                var synonyms = ingredients.SelectMany(i => i.Synonym?.Split(',') ?? Array.Empty<string>()).Distinct().ToList();

                // 將 SelectedIngredients 排序
                var sortedIngredientIds = recipeIngredients.Select(ri => ri.IngredientId ?? 0).ToList();
                sortedIngredientIds.Sort();

                // 創建 DTO
                var recipeDto = new RecipeDto
                {
                    RecipeId = recipe.RecipeId,
                    RecipeName = recipe.RecipeName,
                    UserId = recipe.UserId,
                    IsCustom = recipe.IsCustom,
                    Restriction = recipe.Restriction,
                    WestEast = recipe.WestEast,
                    Category = recipe.Category,
                    DetailedCategory = recipe.DetailedCategory,
                    Steps = recipe.Steps,
                    Seasoning = recipe.Seasoning,
                    Visibility = recipe.Visibility,
                    PhotoName = recipe.Photo,
                    Status = (bool)recipe.Status,
                    Time = recipe.Time,
                    Description = recipe.Description,
                    SelectedIngredients = sortedIngredientIds,
                    SelectedIngredientNames = ingredients.Select(i => i.IngredientName).ToList(),
                    IngredientQuantities = recipeIngredients.GroupBy(ri => ri.IngredientId ?? 0)
                                                              .ToDictionary(g => g.Key, g => g.First().Quantity ?? 0M),
                    IngredientUnits = ingredients.ToDictionary(i => i.IngredientId, i => i.Unit ?? string.Empty),
                    Synonyms = synonyms
                };

                recipeDtos.Add(recipeDto);
            }

            return Ok(recipeDtos);
        }

        // GET: api/Recipes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            // 手動查詢與該 Recipe 關聯的 RecipeIngredients
            var recipeIngredients = await _context.RecipeIngredients
                .Where(ri => ri.RecipeId == recipe.RecipeId)
                .ToListAsync();

            // 查詢對應的食材
            var ingredientIds = recipeIngredients.Select(ri => ri.IngredientId).Distinct().ToList();
            var ingredients = await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.IngredientId))
                .ToListAsync();

            // 查詢同義字
            var synonyms = ingredients.SelectMany(i => i.Synonym?.Split(',') ?? Array.Empty<string>()).Distinct().ToList();

            var sortedIngredientIds = recipeIngredients.Select(ri => ri.IngredientId ?? 0).ToList();
            sortedIngredientIds.Sort();

            // 創建 DTO
            var recipeDto = new RecipeDto
            {
                RecipeId = recipe.RecipeId,
                RecipeName = recipe.RecipeName,
                UserId = recipe.UserId,
                IsCustom = recipe.IsCustom,
                Restriction = recipe.Restriction,
                WestEast = recipe.WestEast,
                Category = recipe.Category,
                DetailedCategory = recipe.DetailedCategory,
                Steps = recipe.Steps,
                Seasoning = recipe.Seasoning,
                Visibility = recipe.Visibility,
                PhotoName = recipe.Photo,
                Status = (bool)recipe.Status,
                Time = recipe.Time,
                Description = recipe.Description,
                SelectedIngredients = sortedIngredientIds,
                SelectedIngredientNames = ingredients.Select(i => i.IngredientName).ToList(),
                IngredientQuantities = recipeIngredients.GroupBy(ri => ri.IngredientId ?? 0)
                                                          .ToDictionary(g => g.Key, g => g.First().Quantity ?? 0M),
                IngredientUnits = ingredients.ToDictionary(i => i.IngredientId, i => i.Unit ?? string.Empty),
                Synonyms = synonyms
            };

            return Ok(recipeDto);
        }

        // PUT: api/Recipes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, [FromForm] RecipeDto recipeDto, IFormFile? photo)
        {
            if (id != recipeDto.RecipeId)
            {

                return BadRequest(new { message = "修改食譜資訊失敗" });
            }

            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            // Update recipe properties
            recipe.RecipeName = recipeDto.RecipeName;
            recipe.UserId = recipeDto.UserId;
            recipe.IsCustom = recipeDto.IsCustom;
            recipe.Restriction = recipeDto.Restriction;
            recipe.WestEast = recipeDto.WestEast;
            recipe.Category = recipeDto.Category;
            recipe.DetailedCategory = recipeDto.DetailedCategory;
            recipe.Steps = recipeDto.Steps;
            recipe.Seasoning = recipeDto.Seasoning;
            recipe.Visibility = recipeDto.Visibility;
            recipe.Status = recipeDto.Status;
            recipe.Time = recipeDto.Time;
            recipe.Description = recipeDto.Description;

            // 如果有上傳新的圖片，儲存圖片
            if (photo != null)
            {
                // 定義圖片儲存的路徑
                var imagePath = Path.Combine("wwwroot/images/recipe", photo.FileName);

                // 儲存圖片到指定位置
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // 更新圖片名稱到資料庫
                recipe.Photo = photo.FileName;
            }

            _context.Entry(recipe).State = EntityState.Modified;

            // Update RecipeIngredients
            var existingIngredients = await _context.RecipeIngredients
                .Where(ri => ri.RecipeId == recipe.RecipeId)
                .ToListAsync();

            // Remove ingredients that are no longer associated
            foreach (var existingIngredient in existingIngredients)
            {
                if (!recipeDto.SelectedIngredients.Contains(existingIngredient.IngredientId ?? 0))
                {
                    _context.RecipeIngredients.Remove(existingIngredient);
                }
            }

            // Add or update ingredients
            foreach (var ingredientId in recipeDto.SelectedIngredients)
            {
                var existingIngredient = existingIngredients.FirstOrDefault(ri => ri.IngredientId == ingredientId);
                if (existingIngredient == null)
                {
                    // Add new ingredient
                    var recipeIngredient = new RecipeIngredient
                    {
                        RecipeId = recipe.RecipeId,
                        IngredientId = ingredientId,
                        Quantity = recipeDto.IngredientQuantities.ContainsKey(ingredientId) ? recipeDto.IngredientQuantities[ingredientId] : 0M
                    };
                    _context.RecipeIngredients.Add(recipeIngredient);
                }
                else
                {
                    // Update existing ingredient quantity
                    existingIngredient.Quantity = recipeDto.IngredientQuantities.ContainsKey(ingredientId) ? recipeDto.IngredientQuantities[ingredientId] : 0M;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "修改食譜資訊成功" });
        }


        // POST: api/Recipes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [RequestSizeLimit(104857600)] // 100 MB
        public async Task<ActionResult<Recipe>> PostRecipe([FromForm] RecipeDto recipeDto, IFormFile photo)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }
            // Custom validation
            if (string.IsNullOrWhiteSpace(recipeDto.RecipeName))
            {
                return BadRequest("食譜名稱不能為空");
            }

            if (recipeDto.SelectedIngredients == null || !recipeDto.SelectedIngredients.Any())
            {
                return BadRequest("至少選擇一樣食材");
            }

            if (string.IsNullOrWhiteSpace(recipeDto.Category))
            {
                return BadRequest("類別需被選擇");
            }
            // Save photo
            string photoFileName = null;
            if (photo != null && photo.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/recipe");
                photoFileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName).Replace(" ", "_")}";
                string filePath = Path.Combine(uploadsFolder, photoFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
            }
            if (photo == null)
            {
                return BadRequest("必須上傳一張食譜照片");
            }

            Console.WriteLine(photoFileName);
            // Create new Recipe entity
            var recipe = new Recipe
            {
                RecipeName = recipeDto.RecipeName,
                UserId = recipeDto.UserId,
                IsCustom = recipeDto.IsCustom,
                Restriction = recipeDto.Restriction,
                WestEast = recipeDto.WestEast,
                Category = recipeDto.Category,
                DetailedCategory = recipeDto.DetailedCategory,
                Steps = recipeDto.Steps,
                Seasoning = recipeDto.Seasoning,
                Visibility = recipeDto.Visibility,
                Photo = photoFileName,
                Status = recipeDto.Status,
                Time = recipeDto.Time,
                Description = recipeDto.Description,
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            // Handle selected ingredients and their quantities
            if (recipeDto.SelectedIngredients != null && recipeDto.IngredientQuantities != null)
            {
                var ingredientIds = recipeDto.SelectedIngredients.Distinct().ToList();

                foreach (var ingredientId in ingredientIds)
                {
                    if (recipeDto.IngredientQuantities.ContainsKey(ingredientId))
                    {
                        var quantity = recipeDto.IngredientQuantities[ingredientId];

                        var recipeIngredient = new RecipeIngredient
                        {
                            RecipeId = recipe.RecipeId,
                            IngredientId = ingredientId,
                            Quantity = quantity
                        };
                        _context.RecipeIngredients.Add(recipeIngredient);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetRecipe", new { id = recipe.RecipeId }, recipe);
        }

        // DELETE: api/Recipes/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteRecipe(int id)
        //{
        //    var recipe = await _context.Recipes.FindAsync(id);
        //    if (recipe == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Recipes.Remove(recipe);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
        // PATCH: api/Recipes/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateRecipeStatus(int id, [FromBody] RecipeStatusDto recipeStatusDto)
        {
            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == id);
            if (recipe == null)
            {
                return NotFound(new { message = "找不到該食譜" });
            }

            // 更新狀態
            recipe.Status = recipeStatusDto.Status;

            // 將更新的狀態應用到資料庫中
            try
            {
                _context.Entry(recipe).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "成功更新食譜狀態" });
        }
        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
        }
    }
}
