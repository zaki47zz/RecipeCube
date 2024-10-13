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

        
        // 讀取所有商品 包含類別
        // GET: api/productsANDcategory
        [HttpGet("ProductsNcategory")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsNcategory()
        {
            var result = await _context.Products
                .GroupJoin(
                _context.Ingredients,
                p => p.IngredientId,    // 產品的 IngredientId
                i => i.IngredientId,    // 類別的 IngredientId
                (p, ingredient) => new { p, ingredient } // 返回一個包含產品 (p) 和對應集合 (ingredient) 的匿名型別
                )
                .SelectMany(
                pg => pg.ingredient.DefaultIfEmpty(), // 左外連接
                (pg, ingredient) => new ProductDTO {   // 為每個產品和它的成分創建 ProductDTO
                    ProductId = pg.p.ProductId,
                    ProductName = pg.p.ProductName,
                    IngredientId = pg.p.IngredientId,
                    Price = pg.p.Price,
                    Stock = pg.p.Stock,
                    Status = pg.p.Status,
                    Photo = pg.p.Photo,
                    Category = ingredient.Category,
                    unit = ingredient.Unit,
                    UnitQuantity = pg.p.UnitQuantity,
                    Description = pg.p.Description
                    }
                )
                .ToListAsync();

            return Ok(result);
        }

        // 讀取單一商品 包含類別
        // GET: api/productsANDcategory/1
        [HttpGet("ProductsNcategory/{id}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsNcategory(int id)
        {
            var result = await _context.Products
                .Where(p=>p.ProductId == id)
                .GroupJoin(
                _context.Ingredients,
                p => p.IngredientId,    // 產品的 IngredientId
                i => i.IngredientId,    // 類別的 IngredientId
                (p, ingredient) => new { p, ingredient } // 返回一個包含產品 (p) 和對應集合 (ingredient) 的匿名型別
                )
                .SelectMany(
                pg => pg.ingredient.DefaultIfEmpty(), // 左外連接
                (pg, ingredient) => new ProductDTO
                {   // 為每個產品和它的成分創建 ProductDTO
                    ProductId = pg.p.ProductId,
                    ProductName = pg.p.ProductName,
                    IngredientId = pg.p.IngredientId,
                    Price = pg.p.Price,
                    Stock = pg.p.Stock,
                    Status = pg.p.Status,
                    Photo = pg.p.Photo,
                    Category = ingredient.Category,
                    unit = ingredient.Unit,
                    UnitQuantity = pg.p.UnitQuantity,
                    Description = pg.p.Description
                }
                )
                .FirstOrDefaultAsync();

            if(result == null )
            {
                return NotFound();
            }

            return Ok(result);
        }


        // 讀取商品類別 並讀出每類商品數量
        // GET: api/GetProductsCategory
        [HttpGet("GetProductsCategory")]
        public async Task<ActionResult<IEnumerable<ProductsCategoryDTO>>> GetProductsCategory()
        {
            var result = await _context.Products
                .GroupJoin(
                _context.Ingredients,
                p => p.IngredientId,    // 產品的 IngredientId
                i => i.IngredientId,    // 類別的 IngredientId
                (p, ingredient) => new { p, ingredient } // 返回一個包含產品 (p) 和對應集合 (ingredient) 的匿名型別
                )
                .SelectMany(
                pg => pg.ingredient.DefaultIfEmpty(), // 左外連接
                (pg, ingredient) => new ProductsCategoryDTO
                {   // 為每個產品和它的成分創建 ProductsCategory
                    ProductId = pg.p.ProductId,
                    Category = ingredient.Category,
                }
                )
                .ToListAsync();

           var categories = result
                .GroupBy(p=>p.Category) //案類別分組
                .Select(g=>new ProductsCategoryDTO // 建立新 DTO
                { 
                    Category = g.Key,
                    Count = g.Count(),
                }).ToList();

            return Ok(categories);
        }

        //=============================================================================

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
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
