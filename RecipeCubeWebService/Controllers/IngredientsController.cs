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
    public class IngredientsController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public IngredientsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: api/Ingredients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
        {
            return await _context.Ingredients.ToListAsync();
        }

        // GET: api/Ingredients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return ingredient;
        }

        // PUT: api/Ingredients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIngredient(int id, Ingredient ingredient)
        {
            if (id != ingredient.IngredientId)
            {
                return BadRequest();
            }

            _context.Entry(ingredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Ingredients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [RequestSizeLimit(104857600)] // 100 MB
        public async Task<ActionResult<Ingredient>> PostIngredient([FromForm] IngredientDTO ingredientDTO)
        {
            //把Photo抓出來處理
            var photo = ingredientDTO.Photo;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }
            // Custom validation
            if (string.IsNullOrWhiteSpace(ingredientDTO.IngredientName))
            {
                return BadRequest("食材不能為空");
            }

            if (string.IsNullOrWhiteSpace(ingredientDTO.Category))
            {
                return BadRequest("類別需被選擇");
            }

            if (string.IsNullOrWhiteSpace(ingredientDTO.IngredientName))
            {
                return BadRequest("食材不能為空");
            }
            if (photo == null)
            {
                return BadRequest("必須上傳一張食材照片");
            }

            // Save photo
            string photoFileName = null;
            if (photo.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ingredient");
                photoFileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName).Replace(" ", "_")}";
                string filePath = Path.Combine(uploadsFolder, photoFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
            }

            // Create new Recipe entity
            var ingredient = new Ingredient
            {
                IngredientName = ingredientDTO.IngredientName,
                Category = ingredientDTO.Category,
                Synonym = ingredientDTO.Synonym,
                ExpireDay = ingredientDTO.ExpireDay,
                Unit = ingredientDTO.Unit,
                Gram = ingredientDTO.Gram,
                Photo = photoFileName,
            };

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIngredient", new { id = ingredient.IngredientId }, ingredient);
        }

        // DELETE: api/Ingredients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.IngredientId == id);
        }
    }
}
