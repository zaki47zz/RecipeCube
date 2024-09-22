using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RecipeIngredientsController : Controller
    {
        private readonly RecipeCubeContext _context;

        public RecipeIngredientsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: Admin/RecipeIngredients
        public async Task<IActionResult> Index()
        {
            var recipeIngredients = await _context.RecipeIngredients.ToListAsync();
            return View(recipeIngredients);
        }

        //public async Task<IActionResult> RecipeIngredientIndexPartial()
        //{
        //    var recipeIngredients = await _context.RecipeIngredients.ToListAsync();
        //    return PartialView("_RecipeIngredientIndexPartial", recipeIngredients);
        //}
        public async Task<IActionResult> RecipeIngredientIndexPartial()
        {
            var recipeIngredients = await _context.RecipeIngredients.ToListAsync();

            var recipeIds = recipeIngredients.Select(ri => ri.RecipeId).Distinct().ToList();
            var recipes = await _context.Recipes
                .Where(r => recipeIds.Contains(r.RecipeId))
                .ToListAsync();

            var ingredientIds = recipeIngredients.Select(ri => ri.IngredientId).Distinct().ToList();
            var ingredients = await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.IngredientId))
                .ToListAsync();

            var viewModel = recipeIngredients.Select(ri => new RecipeIngredientViewModel
            {
                RecipeIngredientId = ri.RecipeIngredientId,
                RecipeId = ri.RecipeId,
                IngredientId = ri.IngredientId,
                Quantity = ri.Quantity,
                RecipeName = recipes.FirstOrDefault(r => r.RecipeId == ri.RecipeId)?.RecipeName,
                IngredientName = ingredients.FirstOrDefault(i => i.IngredientId == ri.IngredientId)?.IngredientName,
                Unit = ingredients.FirstOrDefault(i => i.IngredientId == ri.IngredientId)?.Unit // 食材單位
            }).ToList();

            return PartialView("_RecipeIngredientIndexPartial", viewModel);
        }


        //// GET: Admin/RecipeIngredients/Details/5
        //public async Task<IActionResult> DetailsPartial(int id)
        //{
        //    var recipeIngredient = await _context.RecipeIngredients
        //        .FirstOrDefaultAsync(m => m.RecipeIngredientId == id);

        //    if (recipeIngredient == null)
        //    {
        //        return NotFound();
        //    }

        //    return PartialView("_DetailsPartial", recipeIngredient);
        //}

        // GET: Admin/RecipeIngredients/Details/5
        public async Task<IActionResult> DetailsPartial(int id)
        {
            try
            {
                // 查詢 RecipeIngredient
                var recipeIngredient = await _context.RecipeIngredients.FirstOrDefaultAsync(m => m.RecipeIngredientId == id);
                if (recipeIngredient == null)
                {
                    return NotFound();
                }

                // 查詢對應的 Recipe 名稱
                var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == recipeIngredient.RecipeId);
                if (recipe == null)
                {
                    throw new Exception("Recipe not found.");
                }

                // 查詢對應的 Ingredient 名稱
                var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.IngredientId == recipeIngredient.IngredientId);
                if (ingredient == null)
                {
                    throw new Exception("Ingredient not found.");
                }

                // 創建 ViewModel 並填充資料
                var viewModel = new RecipeIngredientViewModel
                {
                    RecipeIngredientId = recipeIngredient.RecipeIngredientId,
                    RecipeId = recipeIngredient.RecipeId,
                    IngredientId = recipeIngredient.IngredientId,
                    Quantity = recipeIngredient.Quantity,
                    RecipeName = recipe.RecipeName,
                    IngredientName = ingredient.IngredientName,
                    Unit = ingredient.Unit,
                };

                return PartialView("_DetailsPartial", viewModel);
            }
            catch (Exception ex)
            {
                // 記錄異常
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }



        // GET: Admin/RecipeIngredients/CreatePartial
        public async Task<IActionResult> CreatePartial()
        {
            // 加載可用的食譜和食材
            ViewBag.Recipes = await _context.Recipes.Select(r => new { r.RecipeId, r.RecipeName }).ToListAsync();
            ViewBag.Ingredients = await _context.Ingredients.Select(i => new { i.IngredientId, i.IngredientName, i.Unit }).ToListAsync();

            return PartialView("_CreatePartial", new RecipeIngredientViewModel());
        }

        // POST: Admin/RecipeIngredients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeId,IngredientId,Quantity")] RecipeIngredientViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var recipeIngredient = new RecipeIngredient
                {
                    RecipeId = viewModel.RecipeId,
                    IngredientId = viewModel.IngredientId,
                    Quantity = viewModel.Quantity
                };

                _context.Add(recipeIngredient);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            // 如果驗證失敗，重新加載可用的食譜和食材，並返回表單
            ViewBag.Recipes = await _context.Recipes.Select(r => new { r.RecipeId, r.RecipeName }).ToListAsync();
            ViewBag.Ingredients = await _context.Ingredients.Select(i => new { i.IngredientId, i.IngredientName, i.Unit }).ToListAsync();

            return PartialView("_CreatePartial", viewModel);
        }

        // GET: Admin/RecipeIngredients/EditPartial/5
        //public async Task<IActionResult> EditPartial(int id)
        //{
        //    var recipeIngredient = await _context.RecipeIngredients.FindAsync(id);

        //    if (recipeIngredient == null)
        //    {
        //        return NotFound();
        //    }

        //    return PartialView("_EditPartial", recipeIngredient);
        //}
        public async Task<IActionResult> EditPartial(int id)
        {
            var recipeIngredient = await _context.RecipeIngredients.FindAsync(id);

            if (recipeIngredient == null)
            {
                return NotFound();
            }

            // 查詢對應的 Recipe 和 Ingredient 名稱
            var recipe = await _context.Recipes
                .Where(r => r.RecipeId == recipeIngredient.RecipeId)
                .Select(r => new { r.RecipeId, r.RecipeName })
                .FirstOrDefaultAsync();

            var ingredient = await _context.Ingredients
                .Where(i => i.IngredientId == recipeIngredient.IngredientId)
                .Select(i => new { i.IngredientId, i.IngredientName, i.Unit })
                .FirstOrDefaultAsync();

            // 創建 ViewModel 並將數據傳入
            var viewModel = new RecipeIngredientViewModel
            {
                RecipeIngredientId = recipeIngredient.RecipeIngredientId,
                RecipeId = recipeIngredient.RecipeId,
                IngredientId = recipeIngredient.IngredientId,
                Quantity = recipeIngredient.Quantity,
                RecipeName = recipe?.RecipeName, // 使用三元運算符來避免 null 問題
                IngredientName = ingredient?.IngredientName,
                Unit = ingredient?.Unit, // 設置單位
            };

            // 加入從數據庫獲取的可用食譜和食材名稱
            ViewBag.Recipes = await _context.Recipes.Select(r => new { r.RecipeId, r.RecipeName }).ToListAsync();
            ViewBag.Ingredients = await _context.Ingredients.Select(i => new { i.IngredientId, i.IngredientName, i.Unit }).ToListAsync();

            return PartialView("_EditPartial", viewModel);
        }


        // POST: Admin/RecipeIngredients/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecipeIngredientId,RecipeId,IngredientId,Quantity")] RecipeIngredient recipeIngredient)
        {
            int showid = id;
            int showRid = recipeIngredient.RecipeIngredientId;
            Console.WriteLine(showid);
            Console.WriteLine(showRid);

            if (id != recipeIngredient.RecipeIngredientId)
            {
                return Json(new { success = false, error = "ID不符合!" });
            }
            
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    error = "資料無效!",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                _context.Update(recipeIngredient);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Json(new { success = false, error = "資料更新失敗，請稍後再試!", exception = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "發生未知錯誤!", exception = ex.Message });
            }
        }


        public async Task<IActionResult> DeletePartial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipeIngredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(m => m.RecipeIngredientId == id);

            if (recipeIngredient == null)
            {
                return NotFound();
            }

            // 查詢 Recipe 和 Ingredient 的名稱
            var recipe = await _context.Recipes
                .Where(r => r.RecipeId == recipeIngredient.RecipeId)
                .Select(r => new { r.RecipeId, r.RecipeName })
                .FirstOrDefaultAsync();

            var ingredient = await _context.Ingredients
                .Where(i => i.IngredientId == recipeIngredient.IngredientId)
                .Select(i => new { i.IngredientId, i.IngredientName, i.Unit })
                .FirstOrDefaultAsync();

            // 創建 ViewModel 並將資料填充
            var viewModel = new RecipeIngredientViewModel
            {
                RecipeIngredientId = recipeIngredient.RecipeIngredientId,
                RecipeId = recipe?.RecipeId,
                IngredientId = ingredient?.IngredientId,
                Quantity = recipeIngredient.Quantity,
                RecipeName = recipe?.RecipeName,
                IngredientName = ingredient?.IngredientName,
                Unit = ingredient?.Unit
            };

            return PartialView("_DeletePartial", viewModel);
        }


        // POST: Admin/RecipeIngredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var recipeIngredient = await _context.RecipeIngredients.FindAsync(id);
                if (recipeIngredient != null)
                {
                    _context.RecipeIngredients.Remove(recipeIngredient);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "成功刪除!" });
                }
                return Json(new { success = false, error = "資料不存在!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { success = false, error = "發生錯誤!" });
            }
        }

        private bool RecipeIngredientExists(int id)
        {
            return _context.RecipeIngredients.Any(e => e.RecipeIngredientId == id);
        }
    }
}
