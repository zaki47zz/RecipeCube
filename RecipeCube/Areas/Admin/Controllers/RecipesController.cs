using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RecipesController : Controller
    {
        private readonly RecipeCubeContext _context;

        public RecipesController(RecipeCubeContext context)
        {
            _context = context;
        }

        //取得食材
        public async Task<IActionResult> GetIngredients()
        {
            var ingredients = await _context.Ingredients
                                            .Select(
                i => new { i.IngredientId, i.IngredientName , i.Category})
                                            .ToListAsync();

            return Json(ingredients);
        }
        // 搜尋食材
        public IActionResult SearchIngredients(string query)
        {
            var ingredients = _context.Ingredients.AsQueryable();

            // 如果有 query，則進行搜尋
            if (!string.IsNullOrEmpty(query))
            {
                ingredients = ingredients.Where(i => i.IngredientName.Contains(query) || i.Synonym.Contains(query));
            }

            // 如果沒有 query，則返回前10個預設食材
            // 返回所有食材，並按分類分組
            var result = ingredients
                .Select(i => new IngredientViewModel
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.IngredientName,
                    Category = i.Category  // 包含食材的類別
                })
                .ToList();

            return Json(result);  // 以 JSON 格式返回搜尋結果
        }

        // GET: Admin/Recipes
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Recipes.ToListAsync());
        //}

        public async Task<IActionResult> RecipeIndexPartial()
        {
            var recipes = await _context.Recipes.ToListAsync();
            return PartialView("_RecipeIndexPartial", recipes);
        }


        // GET: Admin/Recipes/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var recipe = await _context.Recipes
        //        .FirstOrDefaultAsync(m => m.RecipeId == id);
        //    if (recipe == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(recipe);
        //}
        // 新增的部分：RecipesController.cs
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial", recipe);
        }
        // GET: Admin/Recipes/CreatePartial
        public IActionResult CreatePartial()
        {
            // 從資料庫取得所有可用的食材
            var availableIngredients = _context.Ingredients
                .Select(i => new IngredientViewModel
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.IngredientName
                }).ToList();

            var model = new RecipeViewModel
            {
                AvailableIngredients = availableIngredients,
                SelectedIngredients = new List<int>()  // 預設空的選擇清單
            };

            return PartialView("_CreatePartial", model);
        }

        // POST: Admin/Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeName,UserId,IsCustom,Restriction,WestEast,Category,DetailedCategory,Steps,Seasoning,Visibility,Photo,Status,SelectedIngredients,IngredientQuantities")] RecipeViewModel model)
        {
            bool valid = true;
            if (ModelState.IsValid)
            {
                // 創建新食譜
                var recipe = new Recipe
                {
                    RecipeName = model.RecipeName,
                    UserId = model.UserId,
                    IsCustom = model.IsCustom,
                    Restriction = model.Restriction,
                    WestEast = model.WestEast,
                    Category = model.Category,
                    DetailedCategory = model.DetailedCategory,
                    Steps = model.Steps,
                    Seasoning = model.Seasoning,
                    Visibility = model.Visibility,
                    Photo = model.Photo,
                    Status = model.Status
                };

                _context.Add(recipe);
                await _context.SaveChangesAsync();

                // 處理選擇的食材和數量
                if (model.SelectedIngredients != null && model.IngredientQuantities != null)
                {
                    foreach (var ingredientId in model.SelectedIngredients)
                    {
                        if (model.IngredientQuantities.ContainsKey(ingredientId))
                        {
                            var quantity = model.IngredientQuantities[ingredientId];
                            var recipeIngredient = new RecipeIngredient
                            {
                                RecipeId = recipe.RecipeId,
                                IngredientId = ingredientId,
                                Quantity = quantity
                            };
                            _context.RecipeIngredients.Add(recipeIngredient);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = "Model validation errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });
            }

            Console.WriteLine(valid);
            // 如果 ModelState 無效，重新載入可用的食材
            model.AvailableIngredients = _context.Ingredients
                .Select(i => new IngredientViewModel
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.IngredientName
                }).ToList();

            return PartialView("_CreatePartial", model);
        }


        // GET: Admin/Recipes/Edit/5
        public async Task<IActionResult> EditPartial(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return PartialView("_EditPartial", recipe);
        }

        // POST: Admin/Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, [Bind("RecipeId,RecipeName,UserId,IsCustom,Restriction,WestEast,Category,DetailedCategory,Steps,Seasoning,Visibility,Photo,Status")] Recipe recipe)
        {
            if (id != recipe.RecipeId)
            {
                return new JsonResult(new { success = false, error = "ID不符合!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    _context.SaveChanges();
                    return new JsonResult(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return new JsonResult(new { success = false, error = "發生錯誤!" });
                }
            }

            return new JsonResult(new
            {
                success = false,
                error = "資料無效!",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }





        // GET: Admin/Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Admin/Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
        }
    }
}
