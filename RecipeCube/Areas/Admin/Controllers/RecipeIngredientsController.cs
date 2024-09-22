using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            return View(await _context.RecipeIngredients.ToListAsync());
        }

        public async Task<IActionResult> RecipeIngredientIndexPartial()
        {
            //讀取資料庫內容
            var recipeIngredients = await _context.RecipeIngredients.ToListAsync();
            //這裡把資列庫內容塞進你的Partial頁面並反還這個頁面給Action調用者
            return PartialView("_RecipeIngredientIndexPartial", recipeIngredients);
        }

        // GET: Admin/RecipeIngredients/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(recipeIngredient);
        }

        // GET: Admin/RecipeIngredients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/RecipeIngredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeIngredientId,RecipeId,IngredientId,Quantity")] RecipeIngredient recipeIngredient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipeIngredient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recipeIngredient);
        }

        // GET: Admin/RecipeIngredients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipeIngredient = await _context.RecipeIngredients.FindAsync(id);
            if (recipeIngredient == null)
            {
                return NotFound();
            }
            return View(recipeIngredient);
        }

        // POST: Admin/RecipeIngredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecipeIngredientId,RecipeId,IngredientId,Quantity")] RecipeIngredient recipeIngredient)
        {
            if (id != recipeIngredient.RecipeIngredientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipeIngredient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeIngredientExists(recipeIngredient.RecipeIngredientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(recipeIngredient);
        }

        // GET: Admin/RecipeIngredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(recipeIngredient);
        }

        // POST: Admin/RecipeIngredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipeIngredient = await _context.RecipeIngredients.FindAsync(id);
            if (recipeIngredient != null)
            {
                _context.RecipeIngredients.Remove(recipeIngredient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeIngredientExists(int id)
        {
            return _context.RecipeIngredients.Any(e => e.RecipeIngredientId == id);
        }
        //index
        public async Task<IActionResult> RecipeIngredientsIndexPartial()
        {
            //讀取資料庫內容
            var RecipeIngredients = await _context.RecipeIngredients.ToListAsync();
            //這裡把資列庫內容塞進你的Partial頁面並反還這個頁面給Action調用者
            return PartialView("_RecipeIngredientIndexPartial", RecipeIngredients);
        }

        //details
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var RecipeIngredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(m => m.RecipeIngredientId == id);

            if (RecipeIngredient == null)
            {
                return NotFound();
            }
            return PartialView("_DetailsPartial", RecipeIngredient);
        }
        public IActionResult CreatePartial()
        {
            return PartialView("_CreatePartial"); //PartialView()直接調用，傳回頁面
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IngredientId,IngredientName,Category,Synonym,ExpireDay,Unit,Gram,Photo")] Ingredient ingredient)
        //包括上面，這整段幾乎沿用你的原內容
        {
            if (ModelState.IsValid)
            {
                _context.Add(RecipeIngredient);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index)); //這行刪掉或註掉
                return Json(new { success = true }); //這行給我們一個Json，Layout有功能需要調用
            }
            return PartialView("_CreatePartial", RecipeIngredient); //這行一樣用PartialView()
        }
        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            // 從資料庫獲取 Ingredient (id == id) 該筆資料
            var RecipeIngredient = await _context.RecipeIngredients.FindAsync(id);

            if (RecipeIngredient == null)
            {
                return NotFound();
            }

            return PartialView("_EditPartial", RecipeIngredient);
        }

        //一樣幾乎沿用你的原內容
        [HttpPost]
        [ValidateAntiForgeryToken]
        //回傳型別改用JsonResult
        public JsonResult Edit(int id, [Bind("IngredientId,IngredientName,Category,Synonym,ExpireDay,Unit,Gram,Photo")] Ingredient ingredient)
        {
            if (id != RecipeIngredient.RecipeIngredientId)
            {
                return new JsonResult(new { success = false, error = "ID不符合!" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(RecipeIngredient);
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
        public async Task<IActionResult> DeletePartial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(m => m.IngredientId == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return PartialView("_DeletePartial", ingredient);
        }

        //一樣幾乎沿用你的原內容
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //回傳型別一樣改用JsonResult
        public JsonResult DeleteConfirmed(int id)
        {
            try
            {
                var ingredient = _context.Ingredients.Find(id);
                if (ingredient != null)
                {
                    _context.Ingredients.Remove(ingredient);
                }
                _context.SaveChanges();
                return new JsonResult(new { success = true, error = "成功刪除!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                return new JsonResult(new { success = false, error = "發生錯誤!" });
            }
        }
    }
}
