using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;
using System.Text.RegularExpressions;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InventoriesController : Controller
    {
        private readonly RecipeCubeContext _context;

        public InventoriesController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: Admin/Inventories
        public async Task<IActionResult> Index()
        {

            return View(await _context.Inventories.ToListAsync());
        }

        public async Task<IActionResult> InventoryIndexPartial()
        {
            // 直接獲取所有需要的資料
            var inventories = await _context.Inventories.ToListAsync();
            var users = await _context.Users.ToListAsync();
            var userGroups = await _context.UserGroups.ToListAsync();
            var ingredients = await _context.Ingredients.ToListAsync();

            // 將 userGroups 和 ingredients 轉換為字典以加速查找
            var userDict = users.ToDictionary(u => u.Id, u => u.UserName);
            var userGroupDict = userGroups.ToDictionary(g => g.GroupId, g => g.GroupName);
            var ingredientDict = ingredients.ToDictionary(i => i.IngredientId, i => i.IngredientName);
            var ingredientUnitDict = ingredients.ToDictionary(i => i.IngredientId, i => i.Unit);

            // 使用字典來快速查找資料
            var viewmodel = inventories.Select(inventory => new InventoryViewModel
            {
                InventoryId = inventory.InventoryId,
                GroupId = inventory.GroupId,
                GroupName = userGroupDict.TryGetValue(inventory.GroupId ?? 0, out var groupName) ? groupName : null,
                UserId = inventory.UserId,
                UserName = userDict.TryGetValue(inventory.UserId ?? string.Empty, out var username) ? username : null,
                IngredientId = inventory.IngredientId,
                IngredientName = ingredientDict.TryGetValue(inventory.IngredientId ?? 0, out var ingredientName) ? ingredientName : null,
                IngredientUnit = ingredientUnitDict.TryGetValue(inventory.IngredientId ?? 0, out var ingredientUnit) ? ingredientUnit : null,
                Quantity = inventory.Quantity,
                ExpiryDate = inventory.ExpiryDate,
                IsExpiring = inventory.IsExpiring,
                Visibility = inventory.Visibility
            }).ToList();

            return PartialView("_InventoryIndexPartial", viewmodel);
        }


        // GET: Admin/Inventories/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var inventory = await _context.Inventories
        //        .FirstOrDefaultAsync(m => m.InventoryId == id);
        //    if (inventory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(inventory);
        //}

        public async Task<IActionResult> DetailsPartial(int id)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventory == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == inventory.UserId);
            var userGroup = await _context.UserGroups.FirstOrDefaultAsync(g => g.GroupId == inventory.GroupId);
            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.IngredientId == inventory.IngredientId);

            if (user == null || userGroup == null || ingredient == null)
            {
                return NotFound();
            }

            var viewmodel = new InventoryViewModel
            {
                InventoryId = inventory.InventoryId,
                GroupId = inventory.GroupId,
                GroupName = userGroup.GroupName,
                UserId = inventory.UserId,
                UserName = user.UserName,
                IngredientId = inventory.IngredientId,
                IngredientName = ingredient.IngredientName,
                Quantity = inventory.Quantity,
                IngredientUnit = ingredient.Unit,
                ExpiryDate = inventory.ExpiryDate,
                IsExpiring = inventory.IsExpiring,
                Visibility = inventory.Visibility
            };

            return PartialView("_DetailsPartial", viewmodel);
        }

        // GET: Admin/Inventories/Create
        public IActionResult CreatePartial()
        {
            var groups = _context.UserGroups.ToList();
            var users = _context.Users.ToList();
            var availableIngredients = _context.Ingredients
            .Select(i => new IngredientViewModel
            {
                IngredientId = i.IngredientId,
                IngredientName = i.IngredientName,
                Unit = i.Unit,
                Category = i.Category
            })
            .ToList();

            var model = new InventoryViewModel
            {
                Groups = groups,
                Users = users,
                AvailableIngredients = availableIngredients
            };
            return PartialView("_CreatePartial", model);
        }

        // 搜尋食材 (供前端使用的 AJAX API)
        public IActionResult SearchIngredients(string query)
        {
            var ingredients = _context.Ingredients.AsQueryable();

            // 如果有查詢關鍵字，根據名稱和同義詞進行搜尋
            if (!string.IsNullOrEmpty(query))
            {
                ingredients = ingredients.Where(i => i.IngredientName.Contains(query) || i.Synonym.Contains(query));
            }
            // 只選擇需要的欄位
            var result = ingredients
                .Select(i => new IngredientViewModel
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.IngredientName,
                    Category = i.Category, 
                    Unit = i.Unit,
                })
                .ToList();

            // 返回 JSON 格式的搜尋結果
            return Json(ingredients);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,UserId,SelectedIngredients,IngredientQuantities,IngredientExpiryDate,IngredientIsExpiring,IngredientVisibility")] InventoryViewModel model)
        {
            ModelState.Remove(nameof(model.Users)); //這個欄位會干擾submit，不知道怎麼解

            if (ModelState.IsValid)
            {
                if (model.SelectedIngredients != null && model.IngredientQuantities != null)
                {
                    var ingredientIds = model.SelectedIngredients.Distinct().ToList();

                    foreach (var ingredientId in ingredientIds)
                    {
                        if (model.IngredientQuantities.TryGetValue(ingredientId, out var quantity) &&
                            model.IngredientExpiryDate.TryGetValue(ingredientId, out var expireDate) &&
                            model.IngredientIsExpiring.TryGetValue(ingredientId, out var isExpiring) &&
                            model.IngredientVisibility.TryGetValue(ingredientId, out var visibility))
                        {
                            var inventory = new Inventory
                            {
                                GroupId = model.GroupId,
                                UserId = model.UserId,
                                IngredientId = ingredientId,
                                Quantity = quantity,
                                ExpiryDate = expireDate,
                                IsExpiring = isExpiring,
                                Visibility = visibility,
                            };
                            _context.Add(inventory);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            // 如果 ModelState 無效，返回錯誤信息
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

            model.AvailableIngredients = _context.Ingredients
                .Select(i => new IngredientViewModel
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.IngredientName,
                    Category = i.Category,
                    Unit = i.Unit
                }).ToList();
            var groups = _context.UserGroups.ToList();
            var users = _context.Users.ToList();
            model.Groups = groups;
            model.Users = users;
            return PartialView("_CreatePartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var inventory = await _context.Inventories.FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventory == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == inventory.UserId);
            var userGroup = await _context.UserGroups.FirstOrDefaultAsync(g => g.GroupId == inventory.GroupId);
            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.IngredientId == inventory.IngredientId);

            if (user == null || userGroup == null || ingredient == null)
            {
                return NotFound();
            }

            var viewmodel = new InventoryViewModel
            {
                InventoryId = inventory.InventoryId,
                GroupId = inventory.GroupId,
                GroupName = userGroup.GroupName,
                UserId = inventory.UserId,
                UserName = user.UserName,
                IngredientId = inventory.IngredientId,
                IngredientName = ingredient.IngredientName,
                Quantity = inventory.Quantity,
                IngredientUnit = ingredient.Unit,
                ExpiryDate = inventory.ExpiryDate,
                IsExpiring = inventory.IsExpiring,
                Visibility = inventory.Visibility
            };

            return PartialView("_EditPartial", viewmodel);
        }

        // POST: Admin/Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, [Bind("InventoryId,GroupId,UserId,IngredientId,Quantity,ExpiryDate,IsExpiring,Visibility")] Inventory inventory)
        {
            if (id != inventory.InventoryId)
            {
                return new JsonResult(new { success = false, error = "ID不符合!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventory);
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

        // GET: Admin/Inventories/Delete/5
        public async Task<IActionResult> DeletePartial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories.FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventory == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == inventory.UserId);
            var userGroup = await _context.UserGroups.FirstOrDefaultAsync(g => g.GroupId == inventory.GroupId);
            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.IngredientId == inventory.IngredientId);

            if (user == null || userGroup == null || ingredient == null)
            {
                return NotFound();
            }

            var viewmodel = new InventoryViewModel
            {
                InventoryId = inventory.InventoryId,
                GroupId = inventory.GroupId,
                GroupName = userGroup.GroupName,
                UserId = inventory.UserId,
                UserName = user.UserName,
                IngredientId = inventory.IngredientId,
                IngredientName = ingredient.IngredientName,
                Quantity = inventory.Quantity,
                IngredientUnit = ingredient.Unit,
                ExpiryDate = inventory.ExpiryDate,
                IsExpiring = inventory.IsExpiring,
                Visibility = inventory.Visibility
            };

            return PartialView("_DeletePartial", viewmodel);
        }

        // POST: Admin/Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            try
            {
                var inventory = _context.Inventories.Find(id);
                if (inventory != null)
                {
                    _context.Inventories.Remove(inventory);
                }
                _context.SaveChanges();
                return new JsonResult(new { success = true, error = "成功刪除!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                return new JsonResult(new { success = false, error = "發生錯誤!" });
            }
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.InventoryId == id);
        }
    }
}
