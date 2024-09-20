using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCube.Areas.Admin.ViewModels;
using RecipeCube.Models;
using System.Text.RegularExpressions;

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
            var viewmodel = Pantries.Select(pantry => new PantryViewModel
            {
                PantryId = pantry.PantryId,
                GroupId = pantry.GroupId,
                UserId = pantry.UserId,
                IngredientId = pantry.IngredientId,
                Quantity = pantry.Quantity,
                Action = pantry.Action,
                Time = pantry.Time
            }).ToList();

            return PartialView("_PantryIndexPartial", viewmodel);
        }

        public async Task<IActionResult> DetailsPartial(int id)
        {
            var pantry = await _context.PantryManagements
                .FirstOrDefaultAsync(m => m.PantryId == id); //抓Button傳來的data-id屬性

            if (pantry == null)
            {
                return NotFound();
            }

            var viewmodel = new PantryViewModel
            {
                PantryId = pantry.PantryId,
                GroupId = pantry.GroupId,
                UserId = pantry.UserId,
                IngredientId = pantry.IngredientId,
                Quantity = pantry.Quantity,
                Action = pantry.Action,
                Time = pantry.Time
            };

            return PartialView("_DetailsPartial", viewmodel);
        }

        private bool PantryManagementExists(int id)
        {
            return _context.PantryManagements.Any(e => e.PantryId == id);
        }
    }
}
