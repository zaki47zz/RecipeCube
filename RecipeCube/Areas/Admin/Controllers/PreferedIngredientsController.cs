using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PreferedIngredientsController : Controller
    {
        private readonly RecipeCubeContext _context;

        public PreferedIngredientsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: Admin/PreferedIngredients
        public async Task<IActionResult> PreferedFoodIndexPartial()
        {
            var preferefdoods = await _context.PreferedIngredients.ToListAsync();
            var viewModel = preferefdoods.Select(preferefdood => new PreferedFoodViewModel
            {
                PerferIngredientId = preferefdood.PreferIngredientId,
                UserId = preferefdood.UserId,
                IngredientId = preferefdood.IngredientId
            }).ToList(); 
            return PartialView("_PreferedFoodIndexPartial", viewModel);
        }

        // GET: Admin/PreferedIngredients/Details/5
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var preferefdood = await _context.PreferedIngredients
                .FirstOrDefaultAsync(m => m.PreferIngredientId == id);
            if (preferefdood == null)
            {
                return NotFound();
            }

            var viewmodel = new PreferedFoodViewModel
            {
                PerferIngredientId = preferefdood.PreferIngredientId,
                UserId = preferefdood.UserId,
                IngredientId = preferefdood.IngredientId
            };
            return PartialView("_DetailsPartial", viewmodel);
        }

        // GET: Admin/PreferedIngredients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PreferedIngredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PerferIngredientId,UserId,IngredientId")] PreferedIngredient preferedIngredient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(preferedIngredient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(preferedIngredient);
        }

        // GET: Admin/PreferedIngredients/Edit/5
        [HttpGet]
        public async Task<IActionResult> EditPartial(int? id)
        {
            var preferefdood = await _context.PreferedIngredients.FindAsync(id);
            if (preferefdood == null)
            {
                return NotFound();
            }
            var viewModel = new PreferedFoodViewModel
            {
                PerferIngredientId = preferefdood.PreferIngredientId,
                UserId = preferefdood.UserId,
                IngredientId = preferefdood.IngredientId
            };
            return PartialView("_EditPartial", viewModel);
        }

        // POST: Admin/PreferedIngredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, [Bind("PerferIngredientId,UserId,IngredientId")] PreferedIngredient preferedIngredient)
        {
            if (id != preferedIngredient.PreferIngredientId)
            {
                return new JsonResult(new { success = false, error = "ID不符合!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(preferedIngredient);
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

        // GET: Admin/PreferedIngredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preferedIngredient = await _context.PreferedIngredients
                .FirstOrDefaultAsync(m => m.PreferIngredientId == id);
            if (preferedIngredient == null)
            {
                return NotFound();
            }

            return View(preferedIngredient);
        }

        // POST: Admin/PreferedIngredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var preferedIngredient = await _context.PreferedIngredients.FindAsync(id);
            if (preferedIngredient != null)
            {
                _context.PreferedIngredients.Remove(preferedIngredient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PreferedIngredientExists(int id)
        {
            return _context.PreferedIngredients.Any(e => e.PreferIngredientId == id);
        }
    }
}
