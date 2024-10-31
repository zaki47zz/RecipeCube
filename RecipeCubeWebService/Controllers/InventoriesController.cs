using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCubeWebService.Models;
using RecipeCubeWebService.DTO;
using Microsoft.VisualBasic;

namespace RecipeCubeWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public InventoriesController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: api/Inventories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventories()
        {
            return await _context.Inventories.ToListAsync();
        }

        // GET: api/Inventories/5596af67-3b2a-4d9a-9687-6d290122fd2b
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<InventoryDTO>>> GetInventory(string userId)
        {
            var userInfo = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                GroupId = u.GroupId,
                UserName = u.UserName
            })
            .FirstOrDefaultAsync();

            // 從結果中提取 GroupId 和 UserName
            var groupId = userInfo?.GroupId;
            var userName = userInfo?.UserName;

            if (groupId == null || userName == null)
            {
                return BadRequest();
            }

            var userInventories = await _context.Inventories
                .Where(i => i.GroupId == groupId)
                .ToListAsync(); //抓該群組id的所有庫存

            var ingredients = await _context.Ingredients.ToListAsync(); //抓所有食材的資料

            //為了計算天數要先抓三天後
            DateOnly todayDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly threeDayAfterDate = todayDate.AddDays(3);


            List<InventoryDTO> inventoryDTOs = new List<InventoryDTO>();

            foreach (Inventory userInventory in userInventories)
            {
                var ingredient = ingredients.Where(i => i.IngredientId == userInventory.IngredientId);

                InventoryDTO inventoryDTO = new InventoryDTO
                {
                    InventoryId = userInventory.InventoryId,
                    GroupId = (int)groupId,
                    UserId = userInventory.UserId,
                    UserName = userName,
                    IngredientId = userInventory.IngredientId,
                    Quantity = userInventory.Quantity,
                    ExpiryDate = userInventory.ExpiryDate,
                    IsExpiring = userInventory.ExpiryDate < threeDayAfterDate && userInventory.ExpiryDate > todayDate ? true : false,
                    IsExpired = userInventory.ExpiryDate < todayDate ? true : false,
                    Visibility = userInventory.Visibility,
                    IngredientName = ingredient.Select(i => i.IngredientName).FirstOrDefault(),
                    Category = ingredient.Select(i => i.Category).FirstOrDefault(),
                    Synonym = ingredient.Select(i => i.Synonym).FirstOrDefault(),
                    Unit = ingredient.Select(i => i.Unit).FirstOrDefault(),
                    Gram = ingredient.Select(i => i.Gram).FirstOrDefault(),
                    Photo = ingredient.Select(i => i.Photo).FirstOrDefault(),
                };


                inventoryDTOs.Add(inventoryDTO);
            }

            return Ok(inventoryDTOs); // Return the list with an OK response
        }


        // PUT: api/Inventories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventory(int id, Inventory inventory)
        {
            if (id != inventory.InventoryId)
            {
                return BadRequest();
            }

            _context.Entry(inventory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(id))
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

        // POST: api/Inventories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
        {
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Inventories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.InventoryId == id);
        }
    }
}
