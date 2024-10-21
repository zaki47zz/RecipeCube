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
    public class PantryManagementsController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public PantryManagementsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: api/PantryManagements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PantryManagement>>> GetPantryManagements()
        {
            return await _context.PantryManagements.ToListAsync();
        }

        // GET: api/PantryManagements/5
        [HttpGet("{userId}")]
        public async Task<ActionResult<PantryManagement>> GetPantryManagement(string userId)
        {
            var userInfo = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                GroupId = u.GroupId,
                UserName = u.UserName
            })
            .FirstOrDefaultAsync();

            var groupId = userInfo?.GroupId;
            var userName = userInfo?.UserName;

            if (groupId == null || userName == null)
            {
                return BadRequest();
            }

            var userPantryManagements = await _context.PantryManagements
                .Where(i => i.GroupId == groupId)
                .ToListAsync(); //抓該群組id的所有庫存

            var ingredients = await _context.Ingredients.ToListAsync();

            List<PantryDTO> pantryDTOs = new List<PantryDTO>();

            foreach (PantryManagement userPantryManagement in userPantryManagements)
            {
                var ingredient = ingredients.Where(i => i.IngredientId == userPantryManagement.IngredientId);

                PantryDTO pantryDTO = new PantryDTO
                {
                    PantryId = userPantryManagement.PantryId,
                    GroupId = (int)groupId,
                    UserId = userPantryManagement.UserId,
                    UserName = userName,
                    IngredientId = userPantryManagement.IngredientId,
                    IngredientName = ingredient.Select(i => i.IngredientName).FirstOrDefault(),
                    Quantity = userPantryManagement.Quantity,
                    Unit = ingredient.Select(i => i.Unit).FirstOrDefault(),
                    Action = userPantryManagement.Action,
                    Time = userPantryManagement.Time,
                };


                pantryDTOs.Add(pantryDTO);
            }

            return Ok(pantryDTOs); // Return the list with an OK response
        }


        // POST: api/PantryManagements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PantryManagement>> PostPantryManagement(PantryManagement pantryManagement)
        {
            _context.PantryManagements.Add(pantryManagement);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}