using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeCube.Models;

namespace RecipeCube.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly RecipeCubeContext _context;

        public HomeController(RecipeCubeContext context)
        {
            _context = context;
        }
        // 要求Roles ="Admin"的使用者登入才能訪問Admin頁面
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {

            var userCount = _context.Users.Count();
            var ingredientCount = _context.Ingredients.Count();
            var recipeCount = _context.Recipes.Count();
            var groupCount = _context.UserGroups.Count();
            var productCount = _context.Products.Count();
            var orderCount = _context.Orders.Count();

            ViewData["UserCount"] = userCount;
            ViewData["IngredientCount"] = ingredientCount;
            ViewData["RecipeCount"] = recipeCount;
            ViewData["GroupCount"] = groupCount;
            ViewData["ProductCount"] = productCount;
            ViewData["OrderCount"] = orderCount;

            return View();
        }
    }
}
