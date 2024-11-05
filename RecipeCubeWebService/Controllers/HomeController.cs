using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCubeWebService.Models;
using RecipeCubeWebService.DTO;


namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public HomeController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: api/<HomeController>
        [HttpGet]
        public async Task<ActionResult<homeDTO>> GetInfo()
        {
            int recipeAmount = await _context.Recipes.CountAsync();
            int ingredientAmount = await _context.Ingredients.CountAsync();
            int GroupAmount = await _context.UserGroups.CountAsync();
            int UserAmount = await _context.Users.CountAsync();
            homeDTO homeDTO = new homeDTO{ 
                RecipeAmount = recipeAmount,
                IngredientAmount = ingredientAmount,
                GroupAmount = GroupAmount,
                UserAmount = UserAmount
            };
            return homeDTO;
        }
    }
}
