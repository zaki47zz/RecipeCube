using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using RecipeCube.Models;
using System.Text.RegularExpressions;
using RecipeCube.Data;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InventoriesController : Controller
    {
        private readonly RecipeCubeContext _context;
        private readonly IMemoryCache _cache;

        public InventoriesController(RecipeCubeContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: Admin/Inventories
        public async Task<IActionResult> Index()
        {
            return View(await GetCachedInventories()); 
        }

        public async Task<IActionResult> InventoryIndexPartial()
        {
            var inventories = await GetCachedInventories();
            var users = await GetCachedUsers();
            var userGroups = await GetCachedUserGroups();
            var ingredients = await GetCachedIngredients();

            var userDict = users.ToDictionary(u => u.Id, u => u.UserName);
            var userGroupDict = userGroups.ToDictionary(g => g.GroupId, g => g.GroupName);
            var ingredientDict = ingredients.ToDictionary(i => i.IngredientId, i => new { i.IngredientName, i.Unit });

            var viewmodel = inventories.Select(inventory => new InventoryViewModel
            {
                InventoryId = inventory.InventoryId,
                GroupId = inventory.GroupId,
                GroupName = userGroupDict.TryGetValue(inventory.GroupId ?? 0, out var groupName) ? groupName : null,
                UserId = inventory.UserId,
                UserName = userDict.TryGetValue(inventory.UserId ?? string.Empty, out var username) ? username : null,
                IngredientId = inventory.IngredientId,
                IngredientName = ingredientDict.TryGetValue(inventory.IngredientId ?? 0, out var ingredient) ? ingredient.IngredientName : null,
                IngredientUnit = ingredient?.Unit,
                Quantity = inventory.Quantity,
                ExpiryDate = inventory.ExpiryDate,
                Visibility = inventory.Visibility
            }).ToList();

            return PartialView("_InventoryIndexPartial", viewmodel);
        }

        private async Task<List<Inventory>> GetCachedInventories()
        {
            return await _cache.GetOrCreateAsync("AllInventories", async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return await _context.Inventories.AsNoTracking().ToListAsync();
            });
        }

        private async Task<List<User>> GetCachedUsers()
        {
            return await _cache.GetOrCreateAsync("AllUsers", async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                return await _context.Users.AsNoTracking().ToListAsync();
            });
        }

        private async Task<List<UserGroup>> GetCachedUserGroups()
        {
            return await _cache.GetOrCreateAsync("AllUserGroups", async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                return await _context.UserGroups.AsNoTracking().ToListAsync();
            });
        }

        private async Task<List<Ingredient>> GetCachedIngredients()
        {
            return await _cache.GetOrCreateAsync("AllIngredients", async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                return await _context.Ingredients.AsNoTracking().ToListAsync();
            });
        }

        public async Task<IActionResult> DetailsPartial(int id)
        {
            var viewModel = await GetInventoryViewModel(id);
            if (viewModel == null)
            {
                return NotFound($"找不到這個庫存ID {id}。");
            }
            return PartialView("_DetailsPartial", viewModel);
        }

        // GET: Admin/Inventories/Create
        public IActionResult CreatePartial()
        {
            var model = new InventoryViewModel
            {
                Groups = _context.UserGroups.AsNoTracking().ToList(),
                Users = _context.Users.AsNoTracking().ToList(),
                AvailableIngredients = _context.Ingredients
                    .AsNoTracking()
                    .Select(i => new IngredientViewModel
                    {
                        IngredientId = i.IngredientId,
                        IngredientName = i.IngredientName,
                        Unit = i.Unit,
                        Category = i.Category
                    })
                    .ToList()
            };
            return PartialView("_CreatePartial", model);
        }

        // 搜尋食材 (供前端使用的 AJAX API)
        public IActionResult SearchIngredients(string query)
        {
            var ingredients = _context.Ingredients.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                ingredients = ingredients.Where(i => i.IngredientName.Contains(query) || i.Synonym.Contains(query));
            }

            var result = ingredients
                .Select(i => new IngredientViewModel
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.IngredientName,
                    Category = i.Category,
                    Unit = i.Unit,
                })
                .Take(20) // 限制結果數量以提高性能
                .ToList();

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,UserId,SelectedIngredients,IngredientQuantities,IngredientExpiryDate,IngredientIsExpiring,IngredientVisibility")] InventoryViewModel model)
        {
            ModelState.Remove(nameof(model.Users));

            if (ModelState.IsValid)
            {
                var inventoriesToAdd = new List<Inventory>();

                if (model.SelectedIngredients != null && model.IngredientQuantities != null)
                {
                    foreach (var ingredientId in model.SelectedIngredients.Distinct())
                    {
                        if (model.IngredientQuantities.TryGetValue(ingredientId, out var quantity) &&
                            model.IngredientExpiryDate.TryGetValue(ingredientId, out var expireDate) &&
                            model.IngredientIsExpiring.TryGetValue(ingredientId, out var isExpiring) &&
                            model.IngredientVisibility.TryGetValue(ingredientId, out var visibility))
                        {
                            inventoriesToAdd.Add(new Inventory
                            {
                                GroupId = model.GroupId,
                                UserId = model.UserId,
                                IngredientId = ingredientId,
                                Quantity = quantity,
                                ExpiryDate = expireDate,
                                Visibility = visibility,
                            });
                        }
                    }
                }
                await _context.Inventories.AddRangeAsync(inventoriesToAdd);
                await _context.SaveChangesAsync();
                _cache.Remove("AllInventories");
                return Json(new { success = true });
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            model.AvailableIngredients = _context.Ingredients
                .AsNoTracking()
                .Select(i => new IngredientViewModel
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.IngredientName,
                    Category = i.Category,
                    Unit = i.Unit
                }).ToList();
            var groups = _context.UserGroups.AsNoTracking().ToList();
            var users = _context.Users.AsNoTracking().ToList();
            model.Groups = groups;
            model.Users = users;
            return PartialView("_CreatePartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var viewModel = await GetInventoryViewModel(id);
            if (viewModel == null)
            {
                return NotFound($"找不到這個庫存ID {id}。");
            }
            return PartialView("_EditPartial", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(int id, [Bind("InventoryId,GroupId,UserId,IngredientId,Quantity,ExpiryDate,IsExpiring,Visibility")] Inventory inventory)
        {
            if (id != inventory.InventoryId)
            {
                return Json(new { success = false, error = "ID不符合!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                    _cache.Remove("AllInventories");
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Json(new { success = false, error = ex });
                }
            }
            return Json(new
            {
                success = false,
                error = "資料無效!",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        public async Task<IActionResult> DeletePartial(int? id)
        {
            if (id == null)
            {
                return NotFound("庫存ID是空值");
            }

            var viewModel = await GetInventoryViewModel(id.Value);
            if (viewModel == null)
            {
                return NotFound($"找不到這個庫存ID {id}。");
            }

            return PartialView("_DeletePartial", viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            try
            {
                var inventory = await _context.Inventories.FindAsync(id);
                if (inventory != null)
                {
                    _context.Inventories.Remove(inventory);
                    await _context.SaveChangesAsync();
                    _cache.Remove("AllInventories");
                    return Json(new { success = true, message = "成功刪除!" });
                }
                return Json(new { success = false, error = "找不到要刪除的庫存項目。" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex });
            }
        }

        private async Task<InventoryViewModel> GetInventoryViewModel(int id)
        {
            var inventory = await _context.Inventories
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.InventoryId == id);

            if (inventory == null)
            {
                return null;
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == inventory.UserId);

            var userGroup = await _context.UserGroups
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GroupId == inventory.GroupId);

            var ingredient = await _context.Ingredients
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.IngredientId == inventory.IngredientId);

            if (user == null || userGroup == null || ingredient == null)
            {
                return null;
            }

            return new InventoryViewModel
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
                Visibility = inventory.Visibility
            };
        }

    private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.InventoryId == id);
        }
    }
}
