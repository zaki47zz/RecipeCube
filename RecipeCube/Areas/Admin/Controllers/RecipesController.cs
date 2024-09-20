using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;
using System.Linq;

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
                await _context.SaveChangesAsync(); // // 確保 RecipeId 已經生成

                // 處理選擇的食材和數量
                if (model.SelectedIngredients != null && model.IngredientQuantities != null)
                {
                    var ingredientIds = model.SelectedIngredients.Distinct().ToList(); // 確保選中的食材是唯一的

                    foreach (var ingredientId in ingredientIds)
                    {
                        if (model.IngredientQuantities.ContainsKey(ingredientId))
                        {
                            var quantity = model.IngredientQuantities[ingredientId];

                            // 檢查是否已經存在該 Recipe 和 Ingredient 的關聯
                            var existingIngredient = await _context.RecipeIngredients
                                .FirstOrDefaultAsync(ri => ri.RecipeId == recipe.RecipeId && ri.IngredientId == ingredientId);

                            if (existingIngredient == null)
                            {
                                // 如果關聯不存在，新增一條新的記錄
                                var recipeIngredient = new RecipeIngredient
                                {
                                    RecipeId = recipe.RecipeId,
                                    IngredientId = ingredientId,
                                    Quantity = quantity
                                };
                                _context.RecipeIngredients.Add(recipeIngredient);
                            }
                            else
                            {
                                // 如果已經存在，更新數量
                                existingIngredient.Quantity = quantity;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();  // 保存 RecipeIngredients 的變更
                return Json(new { success = true });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = "Model validation errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });
            }


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
            // 查詢 Recipe
            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            // 手動查詢與該 Recipe 關聯的 RecipeIngredients
            var recipeIngredients = await _context.RecipeIngredients
                .Where(ri => ri.RecipeId == id)
                .ToListAsync();

            // 手動查詢對應的食材
            var ingredientIds = recipeIngredients.Select(ri => ri.IngredientId).ToList();
            var ingredients = await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.IngredientId))
                .ToListAsync();

            // 創建 ViewModel
            var viewModel = new RecipeViewModel
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
                Photo = recipe.Photo,
                Status = recipe.Status,

                // 使用 SelectedIngredients 保存選擇的食材ID
                SelectedIngredients = recipeIngredients.Select(ri => ri.IngredientId.GetValueOrDefault()).ToList(),


                // 使用 IngredientQuantities 保存選擇的食材和數量對應關係
                IngredientQuantities = recipeIngredients.ToDictionary(ri => ri.IngredientId.GetValueOrDefault(), ri => ri.Quantity.GetValueOrDefault()),

            }; 

            // 查詢所有可用的食材以供選擇
            viewModel.AvailableIngredients = await _context.Ingredients
                .Select(i => new IngredientViewModel
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.IngredientName
                }).ToListAsync();

            return PartialView("_EditPartial", viewModel);
        }

        // POST: Admin/Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(int id, [Bind("RecipeId,RecipeName,UserId,IsCustom,Restriction,WestEast,Category,DetailedCategory,Steps,Seasoning,Visibility,Photo,Status,SelectedIngredients,IngredientQuantities")] RecipeViewModel model)
        {
            if (id != model.RecipeId)
            {
                return new JsonResult(new { success = false, error = "ID 不符合!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 查找原本的食譜
                    var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == id);

                    if (recipe == null)
                    {
                        return new JsonResult(new { success = false, error = "找不到該食譜!" });
                    }

                    // 更新食譜的屬性
                    recipe.RecipeName = model.RecipeName;
                    recipe.UserId = model.UserId;
                    recipe.IsCustom = model.IsCustom;
                    recipe.Restriction = model.Restriction;
                    recipe.WestEast = model.WestEast;
                    recipe.Category = model.Category;
                    recipe.DetailedCategory = model.DetailedCategory;
                    recipe.Steps = model.Steps;
                    recipe.Seasoning = model.Seasoning;
                    recipe.Visibility = model.Visibility;
                    recipe.Photo = model.Photo;
                    recipe.Status = model.Status;

                    // 查詢與該食譜關聯的 RecipeIngredients
                    var existingRecipeIngredients = await _context.RecipeIngredients
                        .Where(ri => ri.RecipeId == id)
                        .ToListAsync();

                    // 更新選擇的食材
                    foreach (var selectedIngredientId in model.SelectedIngredients)
                    {
                        // 查找是否已有這個食材的關聯
                        var existingIngredient = existingRecipeIngredients
                            .FirstOrDefault(ri => ri.IngredientId.HasValue && ri.IngredientId.Value == selectedIngredientId);

                        // 如果該食材有被選擇，則查找對應的數量
                        var quantity = model.IngredientQuantities.ContainsKey(selectedIngredientId)
                            ? model.IngredientQuantities[selectedIngredientId]
                            : 0; // 如果數量未提供，設為 0 或其他預設值

                        if (existingIngredient != null)
                        {
                            // 更新數量
                            existingIngredient.Quantity = quantity;
                        }
                        else
                        {
                            // 如果這個食材尚未關聯，則添加新關聯
                            var newIngredient = new RecipeIngredient
                            {
                                RecipeId = recipe.RecipeId,
                                IngredientId = selectedIngredientId,
                                Quantity = quantity
                            };
                            await _context.RecipeIngredients.AddAsync(newIngredient);
                        }
                    }

                    // 刪除已被移除的食材
                    var removedIngredients = existingRecipeIngredients
                        .Where(ri => !model.SelectedIngredients.Contains(ri.IngredientId.Value))
                        .ToList();

                    foreach (var removedIngredient in removedIngredients)
                    {
                        _context.RecipeIngredients.Remove(removedIngredient);
                    }

                    // 保存變更
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();

                    return new JsonResult(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return new JsonResult(new { success = false, error = "發生錯誤，更新失敗!" });
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
