using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PantryManagementsController : Controller
    {
        private readonly RecipeCubeContext _context;

        public PantryManagementsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: Admin/PantryManagements
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.PantryManagements.ToListAsync());
        //}

        public async Task<IActionResult> PantryIndexPartial()
        {
            var Pantries = await _context.PantryManagements.ToListAsync();
            return PartialView("_PantryIndexPartial", Pantries);
        }

        // GET: Admin/PantryManagements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pantryManagement = await _context.PantryManagements
                .FirstOrDefaultAsync(m => m.PantryId == id);
            if (pantryManagement == null)
            {
                return NotFound();
            }

            return View(pantryManagement);
        }

        // GET: Admin/PantryManagements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PantryManagements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PantryId,GroupId,UserId,IngredientId,Quantity,OutOfStock,Action,Time")] PantryManagement pantryManagement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pantryManagement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pantryManagement);
        }

        // GET: Admin/PantryManagements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pantryManagement = await _context.PantryManagements.FindAsync(id);
            if (pantryManagement == null)
            {
                return NotFound();
            }
            return View(pantryManagement);
        }

        // POST: Admin/PantryManagements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PantryId,GroupId,UserId,IngredientId,Quantity,OutOfStock,Action,Time")] PantryManagement pantryManagement)
        {
            if (id != pantryManagement.PantryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pantryManagement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PantryManagementExists(pantryManagement.PantryId))
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
            return View(pantryManagement);
        }

        // GET: Admin/PantryManagements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pantryManagement = await _context.PantryManagements
                .FirstOrDefaultAsync(m => m.PantryId == id);
            if (pantryManagement == null)
            {
                return NotFound();
            }

            return View(pantryManagement);
        }

        // POST: Admin/PantryManagements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pantryManagement = await _context.PantryManagements.FindAsync(id);
            if (pantryManagement != null)
            {
                _context.PantryManagements.Remove(pantryManagement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PantryManagementExists(int id)
        {
            return _context.PantryManagements.Any(e => e.PantryId == id);
        }
    }
}
