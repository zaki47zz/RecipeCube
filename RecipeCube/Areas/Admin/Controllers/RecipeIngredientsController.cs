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
    }
}
