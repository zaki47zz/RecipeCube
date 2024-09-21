using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExclusiveIngredientsController : Controller
    {
        private readonly RecipeCubeContext _context;

        public ExclusiveIngredientsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: Admin/ExclusiveIngredients
        public async Task<IActionResult> ExclusiveFoodIndexPartial()
        {
            var exclusivefoods = await _context.ExclusiveIngredients.ToListAsync();
            var viewModel = exclusivefoods.Select(exclusivefood => new ExclusiveFoodViewModel
            {
                ExclusiveIngredientId = exclusivefood.ExclusiveIngredientId,
                UserId = exclusivefood.UserId,
                IngredientId = exclusivefood.IngredientId
            });
            return PartialView("_ExclusiveFoodIndexPartial", viewModel);
        }

        // GET: Admin/ExclusiveIngredients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exclusiveIngredient = await _context.ExclusiveIngredients
                .FirstOrDefaultAsync(m => m.ExclusiveIngredientId == id);
            if (exclusiveIngredient == null)
            {
                return NotFound();
            }

            return View(exclusiveIngredient);
        }

        // GET: Admin/ExclusiveIngredients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ExclusiveIngredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExclusiveIngredientId,UserId,IngredientId")] ExclusiveIngredient exclusiveIngredient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exclusiveIngredient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exclusiveIngredient);
        }

        // GET: Admin/ExclusiveIngredients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exclusiveIngredient = await _context.ExclusiveIngredients.FindAsync(id);
            if (exclusiveIngredient == null)
            {
                return NotFound();
            }
            return View(exclusiveIngredient);
        }

        // POST: Admin/ExclusiveIngredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExclusiveIngredientId,UserId,IngredientId")] ExclusiveIngredient exclusiveIngredient)
        {
            if (id != exclusiveIngredient.ExclusiveIngredientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exclusiveIngredient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExclusiveIngredientExists(exclusiveIngredient.ExclusiveIngredientId))
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
            return View(exclusiveIngredient);
        }

        // GET: Admin/ExclusiveIngredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exclusiveIngredient = await _context.ExclusiveIngredients
                .FirstOrDefaultAsync(m => m.ExclusiveIngredientId == id);
            if (exclusiveIngredient == null)
            {
                return NotFound();
            }

            return View(exclusiveIngredient);
        }

        // POST: Admin/ExclusiveIngredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exclusiveIngredient = await _context.ExclusiveIngredients.FindAsync(id);
            if (exclusiveIngredient != null)
            {
                _context.ExclusiveIngredients.Remove(exclusiveIngredient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExclusiveIngredientExists(int id)
        {
            return _context.ExclusiveIngredients.Any(e => e.ExclusiveIngredientId == id);
        }
    }
}
