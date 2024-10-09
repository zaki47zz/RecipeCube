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
    public class ProductsController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public ProductsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/productsANDcategory
        [HttpGet("ProductsNcategory")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsNcategory()
        {
            var result = await _context.Products
                .GroupJoin(
                    _context.Ingredients,
                    p => p.IngredientId,
                    i => i.IngredientId,
                    (p, ig) => new { p, ig }
                )
                .SelectMany(
                    pg => pg.ig.DefaultIfEmpty(), // 左外聯接
                    (pg, ig) => new { pg.p, ig }
                )
                .GroupJoin(
                    _context.RecipeIngredients,
                    pg => pg.ig.IngredientId, // 假設使用 IngredientId 來聯接 RecipeIngredients
                    ri => ri.IngredientId, // 假設 RecipeIngredients 中也有 IngredientId
                    (pg, ri) => new { pg.p, pg.ig, ri }
                )
                .SelectMany(
                    x => x.ri.DefaultIfEmpty(), // 左外聯接
                    (x, ri) => new ProductDTO
                    {
                        ProductId = x.p.ProductId,
                        ProductName = x.p.ProductName,
                        IngredientId = x.p.IngredientId,
                        Price = x.p.Price,
                        Stock = x.p.Stock,
                        Status = x.p.Status,
                        Photo = x.p.Photo,
                        Category = x.ig.Category,
                        unit = x.ig.Unit,
                        Quantity = ri.Quantity,
                        UnitQuantity=x.p.UnitQuantity,
                    }
                )
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
