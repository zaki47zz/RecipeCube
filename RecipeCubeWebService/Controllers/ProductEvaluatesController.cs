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
    public class ProductEvaluatesController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public ProductEvaluatesController(RecipeCubeContext context)
        {
            _context = context;
        }

        // 抓取全部評論
        // GET: api/ProductEvaluates
        [HttpGet("GetProductEvaluateWithUserName")]
        public async Task<ActionResult<IEnumerable<ProductEvaluatesDTO>>> GetProductEvaluateWithUserName()
        {
            // 查詢所有的 ProductEvaluate 並 Join Users 表來取得 UserName
            var productEvaluates = await _context.ProductEvaluates
                .Join(
                    _context.Users,
                    evaluate => evaluate.UserId,
                    user => user.Id,
                    (evaluate, user) => new ProductEvaluatesDTO
                    {
                        EvaluateId = evaluate.EvaluateId,
                        UserId = evaluate.UserId,
                        UserName = user.UserName, // 將 UserName 填入 DTO
                        ProductId = evaluate.ProductId,
                        CommentMessage = evaluate.CommentMessage,
                        CommentStars = evaluate.CommentStars,
                        Date = evaluate.Date
                    }
                )
                .ToListAsync();

            return productEvaluates;
        }



        // 抓取單一評論含評論者名稱
        [HttpGet("GetProductEvaluateWithUserName/{id}")]
        public async Task<ActionResult<ProductEvaluatesDTO>> GetProductEvaluateWithUserName(int id)
        {
            // 從資料庫中查詢 ProductEvaluate
            var productEvaluate = await _context.ProductEvaluates.FindAsync(id);

            if (productEvaluate == null)
            {
                return NotFound();
            }

            // 從 Users 表中查找 UserName
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == productEvaluate.UserId);

            // 將查詢結果轉換為 DTO
            var productEvaluatesDto = new ProductEvaluatesDTO
            {
                EvaluateId = productEvaluate.EvaluateId,
                UserId = productEvaluate.UserId,
                UserName = user?.UserName, // 如果 user 存在則取 UserName，否則為 null
                ProductId = productEvaluate.ProductId,
                CommentMessage = productEvaluate.CommentMessage,
                CommentStars = productEvaluate.CommentStars,
                Date = productEvaluate.Date
            };

            return productEvaluatesDto;
        }




        // GET: api/ProductEvaluates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductEvaluate>>> GetProductEvaluates()
        {
            return await _context.ProductEvaluates.ToListAsync();
        }

        // GET: api/ProductEvaluates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductEvaluate>> GetProductEvaluate(int id)
        {
            var productEvaluate = await _context.ProductEvaluates.FindAsync(id);

            if (productEvaluate == null)
            {
                return NotFound();
            }

            return productEvaluate;
        }

        // PUT: api/ProductEvaluates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductEvaluate(int id, ProductEvaluate productEvaluate)
        {
            if (id != productEvaluate.EvaluateId)
            {
                return BadRequest();
            }

            _context.Entry(productEvaluate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductEvaluateExists(id))
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

        // POST: api/ProductEvaluates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductEvaluate>> PostProductEvaluate(ProductEvaluate productEvaluate)
        {
            _context.ProductEvaluates.Add(productEvaluate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductEvaluate", new { id = productEvaluate.EvaluateId }, productEvaluate);
        }

        // DELETE: api/ProductEvaluates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductEvaluate(int id)
        {
            var productEvaluate = await _context.ProductEvaluates.FindAsync(id);
            if (productEvaluate == null)
            {
                return NotFound();
            }

            _context.ProductEvaluates.Remove(productEvaluate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductEvaluateExists(int id)
        {
            return _context.ProductEvaluates.Any(e => e.EvaluateId == id);
        }
    }
}
