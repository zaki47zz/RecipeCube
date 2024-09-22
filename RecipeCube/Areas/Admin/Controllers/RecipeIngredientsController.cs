using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> RecipeIngredientIndexPartial()
        {
            var recipeIngredients = await _context.RecipeIngredients.ToListAsync();
            return PartialView("_RecipeIngredientIndexPartial", recipeIngredients);
        }

        // GET: Admin/RecipeIngredients/Details/5
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var recipeIngredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(m => m.RecipeIngredientId == id);

            if (recipeIngredient == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial", recipeIngredient);
        }

        // GET: Admin/RecipeIngredients/CreatePartial
        public IActionResult CreatePartial()
        {
            return PartialView("_CreatePartial");
        }

        // POST: Admin/RecipeIngredients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeIngredientId,RecipeId,IngredientId,Quantity")] RecipeIngredient recipeIngredient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipeIngredient);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("_CreatePartial", recipeIngredient);
        }

        // GET: Admin/RecipeIngredients/EditPartial/5
        public async Task<IActionResult> EditPartial(int id)
        {
            var recipeIngredient = await _context.RecipeIngredients.FindAsync(id);

            if (recipeIngredient == null)
            {
                return NotFound();
            }

            return PartialView("_EditPartial", recipeIngredient);
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


        // GET: Admin/RecipeIngredients/DeletePartial/5
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

            return PartialView("_DeletePartial", recipeIngredient);
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
