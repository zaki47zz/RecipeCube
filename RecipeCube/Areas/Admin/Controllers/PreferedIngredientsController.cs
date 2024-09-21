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
                PerferIngredientId = preferefdood.PerferIngredientId,
                UserId = preferefdood.UserId,
                IngredientId = preferefdood.IngredientId
            });
            return PartialView("_PreferedFoodIndexPartial", viewModel);
        }

        // GET: Admin/PreferedIngredients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preferedIngredient = await _context.PreferedIngredients
                .FirstOrDefaultAsync(m => m.PerferIngredientId == id);
            if (preferedIngredient == null)
            {
                return NotFound();
            }

            return View(preferedIngredient);
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preferedIngredient = await _context.PreferedIngredients.FindAsync(id);
            if (preferedIngredient == null)
            {
                return NotFound();
            }
            return View(preferedIngredient);
        }

        // POST: Admin/PreferedIngredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PerferIngredientId,UserId,IngredientId")] PreferedIngredient preferedIngredient)
        {
            if (id != preferedIngredient.PerferIngredientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(preferedIngredient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreferedIngredientExists(preferedIngredient.PerferIngredientId))
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
            return View(preferedIngredient);
        }

        // GET: Admin/PreferedIngredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preferedIngredient = await _context.PreferedIngredients
                .FirstOrDefaultAsync(m => m.PerferIngredientId == id);
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
            return _context.PreferedIngredients.Any(e => e.PerferIngredientId == id);
        }
    }
}
