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
            var userGroups = await _context.UserGroups.ToListAsync();
            var ingredients = await _context.Ingredients.ToListAsync();

            // 將 userGroups 和 ingredients 轉換為字典以加速查找
            var userGroupDict = userGroups.ToDictionary(g => g.GroupId, g => g.GroupName);
            var ingredientDict = ingredients.ToDictionary(i => i.IngredientId, i => i.IngredientName);

            // 使用字典來快速查找資料
            var viewmodel = inventories.Select(inventory => new InventoryViewModel
            {
                InventoryId = inventory.InventoryId,
                GroupId = inventory.GroupId,
                GroupName = userGroupDict.TryGetValue(inventory.GroupId ?? 0, out var groupName) ? groupName : null,
                UserId = inventory.UserId,
                IngredientId = inventory.IngredientId,
                IngredientName = ingredientDict.TryGetValue(inventory.IngredientId ?? 0, out var ingredientName) ? ingredientName : null,
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
            var Inventory = await _context.Inventories
                .FirstOrDefaultAsync(m => m.InventoryId == id);

            if (Inventory == null)
            {
                return NotFound();
            }

            var viewmodel = new InventoryViewModel
            {
                InventoryId = Inventory.InventoryId,
                GroupId = Inventory.GroupId,
                UserId = Inventory.UserId,
                IngredientId = Inventory.IngredientId,
                Quantity = Inventory.Quantity,
                ExpiryDate = Inventory.ExpiryDate,
                IsExpiring = Inventory.IsExpiring,
                Visibility = Inventory.Visibility
            };

            return PartialView("_DetailsPartial", viewmodel);
        }

        // GET: Admin/Inventories/Create
        public IActionResult CreatePartial()
        {
            return PartialView("_CreatePartial");
        }

        // POST: Admin/Inventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryId,GroupId,UserId,IngredientId,Quantity,ExpiryDate,IsExpiring,Visibility")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_CreatePartial", inventory);
        }

        // GET: Admin/Inventories/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var inventory = await _context.Inventories.FindAsync(id);
        //    if (inventory == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(inventory);
        //}

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            var viewmodel = new InventoryViewModel
            {
                InventoryId = inventory.InventoryId,
                GroupId = inventory.GroupId,
                UserId = inventory.UserId,
                IngredientId = inventory.IngredientId,
                Quantity = inventory.Quantity,
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

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventory == null)
            {
                return NotFound();
            }

            var viewmodel = new InventoryViewModel
            {
                InventoryId = inventory.InventoryId,
                GroupId = inventory.GroupId,
                UserId = inventory.UserId,
                IngredientId = inventory.IngredientId,
                Quantity = inventory.Quantity,
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
