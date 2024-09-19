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

        public async Task<IActionResult> PantryIndexPartial()
        {
            var Pantries = await _context.PantryManagements.ToListAsync();
            return PartialView("_PantryIndexPartial", Pantries);
        }

        public async Task<IActionResult> DetailsPartial(int id)
        {
            var pantry = await _context.PantryManagements
                .FirstOrDefaultAsync(m => m.PantryId == id); //抓Button傳來的data-id屬性

            if (pantry == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial", pantry);
        }

        private bool PantryManagementExists(int id)
        {
            return _context.PantryManagements.Any(e => e.PantryId == id);
        }
    }
}
