using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCubeWebService.DTO;
using RecipeCubeWebService.Models;

namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserIngredientsController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public UserIngredientsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // 取得使用者不可食用食材
        // GET: /api/UserIngredients/ExclusiveIngredientsName
        [HttpGet("ExclusiveIngredientsName")]
        public IActionResult GetExclusiveIngredientDetails([FromQuery] Users_Id_IngredientsDTO exi)
        {
            var exclusiveIngredientsDetails = _context.ExclusiveIngredients
             .Include(e =>e.Ingredient) // 載入 Ingredient
            .Where(e => e.UserId == exi.User_Id)
            .Select(e => new
            {
                ExclusiveIngredientId = e.ExclusiveIngredientId,
                ExclusiveIngredientName = e.Ingredient.IngredientName
             })
            .ToList();
            if (!exclusiveIngredientsDetails.Any())
            {
                return NoContent(); // 或者使用 return Ok(); 返回空的結果
            }
            return Ok(new { ExclusiveIngredients = exclusiveIngredientsDetails });
        }

        // 取得使用者偏好食材
        // GET: /api/UserIngredients/PreferedIngrediensName
        [HttpGet("PreferedIngredientsName")]
        public IActionResult GetPreferedIngredientDetails([FromQuery] Users_Id_IngredientsDTO exi)
        {
            var preferredIngredientsDetails = _context.PreferedIngredients
             .Include(e => e.Ingredient) // 載入 Ingredient
            .Where(e => e.UserId == exi.User_Id)
            .Select(e => new
            {
                PreferIngredientId = e.PerferIngredientId,
                PreferIngredientName = e.Ingredient.IngredientName
            })
            .ToList();
            if (!preferredIngredientsDetails.Any())
            {
                return NoContent(); // 或者使用 return Ok(); 返回空的結果
            }
            return Ok(new { PreferredIngredients = preferredIngredientsDetails });
        }













        //// GET: api/ExclusiveIngredients
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ExclusiveIngredient>>> GetExclusiveIngredients()
        //{
        //    return await _context.ExclusiveIngredients.ToListAsync();
        //}  

        //// PUT: api/ExclusiveIngredients/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutExclusiveIngredient(int id, ExclusiveIngredient exclusiveIngredient)
        //{
        //    if (id != exclusiveIngredient.ExclusiveIngredientId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(exclusiveIngredient).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ExclusiveIngredientExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/ExclusiveIngredients
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<ExclusiveIngredient>> PostExclusiveIngredient(ExclusiveIngredient exclusiveIngredient)
        //{
        //    _context.ExclusiveIngredients.Add(exclusiveIngredient);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetExclusiveIngredient", new { id = exclusiveIngredient.ExclusiveIngredientId }, exclusiveIngredient);
        //}

        // DELETE: /api/UserIngredients/ExclusiveIngredientsDelete
        [HttpDelete("ExclusiveIngredientsDelete")]
        public async Task<IActionResult> DeleteExclusiveIngredient(int id)
        {
            var exclusiveIngredient = await _context.ExclusiveIngredients.FindAsync(id);
            if (exclusiveIngredient == null)
            {
                return NotFound();
            }

            _context.ExclusiveIngredients.Remove(exclusiveIngredient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: /api/UserIngredients/PreferedIngrediensDelete
        [HttpDelete("PreferedIngrediensDelete")]
        public async Task<IActionResult> DeletePreferedIngredient(int id)
        {
            var preferedIngredient = await _context.PreferedIngredients.FindAsync(id);
            if (preferedIngredient == null)
            {
                return NotFound();
            }

            _context.PreferedIngredients.Remove(preferedIngredient);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool ExclusiveIngredientExists(int id)
        {
            return _context.ExclusiveIngredients.Any(e => e.ExclusiveIngredientId == id);
        }
    }
}
