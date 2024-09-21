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
            var users = await _context.Users.ToListAsync();
            var userGroups = await _context.UserGroups.ToListAsync();
            var ingredients = await _context.Ingredients.ToListAsync();

            // 將 userGroups 和 ingredients 轉換為字典以加速查找
            var userDict = users.ToDictionary(u => u.Id, u => u.UserName);
            var userGroupDict = userGroups.ToDictionary(g => g.GroupId, g => g.GroupName);
            var ingredientDict = ingredients.ToDictionary(i => i.IngredientId, i => i.IngredientName);
            var ingredientUnitDict = ingredients.ToDictionary(i => i.IngredientId, i => i.Unit);

            var viewmodel = Pantries.Select(pantry => new PantryViewModel
            {
                PantryId = pantry.PantryId,
                GroupId = pantry.GroupId,
                GroupName = userGroupDict.TryGetValue(pantry.GroupId ?? 0, out var groupName) ? groupName : null,
                UserId = pantry.UserId,
                UserName = userDict.TryGetValue(pantry.UserId ?? string.Empty, out var username) ? username : null,
                IngredientId = pantry.IngredientId,
                IngredientName = ingredientDict.TryGetValue(pantry.IngredientId ?? 0, out var ingredientName) ? ingredientName : null,
                IngredientUnit = ingredientUnitDict.TryGetValue(pantry.IngredientId ?? 0, out var ingredientUnit) ? ingredientUnit : null,
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == pantry.UserId);
            var userGroup = await _context.UserGroups.FirstOrDefaultAsync(g => g.GroupId == pantry.GroupId);
            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.IngredientId == pantry.IngredientId);


            if (user == null || userGroup == null || ingredient == null)
            {
                return NotFound();
            }

            var viewmodel = new PantryViewModel
            {
                PantryId = pantry.PantryId,
                GroupId = pantry.GroupId,
                GroupName = userGroup.GroupName,
                UserId = pantry.UserId,
                UserName = user.UserName,
                IngredientId = pantry.IngredientId,
                IngredientName = ingredient.IngredientName,
                IngredientUnit = ingredient.Unit,
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
