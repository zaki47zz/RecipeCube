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
    public class UserGroupsController : ControllerBase
    {
        private readonly RecipeCubeContext _context;

        public UserGroupsController(RecipeCubeContext context)
        {
            _context = context;
        }

        // GET: api/UserGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGroup>>> GetUserGroups()
        {
            return await _context.UserGroups.ToListAsync();
        }

        // GET: api/UserGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserGroup>> GetUserGroup(int id)
        {
            var userGroup = await _context.UserGroups.FindAsync(id);

            if (userGroup == null)
            {
                return NotFound();
            }

            return userGroup;
        }

        // PUT: api/UserGroups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserGroup(int id, UserGroup userGroup)
        {
            if (id != userGroup.GroupId)
            {
                return BadRequest();
            }

            _context.Entry(userGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserGroupExists(id))
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

        // POST: api/UserGroups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserGroup>> PostUserGroup(UserGroup userGroup)
        {
            _context.UserGroups.Add(userGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserGroup", new { id = userGroup.GroupId }, userGroup);
        }

        // DELETE: api/UserGroups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserGroup(int id)
        {
            var userGroup = await _context.UserGroups.FindAsync(id);
            if (userGroup == null)
            {
                return NotFound();
            }

            _context.UserGroups.Remove(userGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/UserGroups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateGroup")]
        public async Task<IActionResult> CreateGroupDTO(CreateGroupDTO userGroupAdd)
        {
            if (userGroupAdd == null) {
                return BadRequest("userGroup沒資料");
            }
            var existingGroup = await _context.UserGroups.SingleOrDefaultAsync(u => u.GroupAdmin == userGroupAdd.group_Admin_Id);
            if (existingGroup != null)  {
                return NotFound(new { Message = "已有群組" });
            }
            Random Number = new Random();
            var GroupAdmin = userGroupAdd.group_Admin_Id;
            var GroupName = userGroupAdd.group_name;
            var GroupInvite = 0;
            int group_Invite = 0;
            Console.WriteLine();
            Console.WriteLine(group_Invite);
            group_Invite = Number.Next(100000000, 999999999);
            var existingGroupInvite = await _context.UserGroups.SingleOrDefaultAsync(u => u.GroupInvite == group_Invite);
            Console.WriteLine();
            Console.WriteLine(group_Invite);
            while (existingGroupInvite != null)
            {
                GroupInvite = group_Invite;
            }
            Console.WriteLine();
            Console.WriteLine(group_Invite);

            var newGroug = new UserGroup
            {
                GroupAdmin = userGroupAdd.group_Admin_Id,
                GroupName = userGroupAdd.group_name,
                GroupInvite = group_Invite,
            };

            _context.UserGroups.Add(newGroug);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "新增成功" });
        }

        private bool UserGroupExists(int id)
        {
            return _context.UserGroups.Any(e => e.GroupId == id);
        }
    }
}
