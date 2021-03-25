using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "administrator")]
    public class GroupsController : ControllerBase
    {
        private readonly UserManagementDataContext context;

        public GroupsController(UserManagementDataContext context)
        {
            this.context = context;
        }
        
        [HttpGet("{groupId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Group>> GetGroup(int groupId)
        {
            var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(group);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetAll()
        {
            return Ok(await context.Groups.ToListAsync());
        }
        
        [HttpGet("{groupId}/groups")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Group>>> GetChildGroups(int groupId)
        {
            var group = await context.Groups.Include(g => g.ChildGroups).FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(group.ChildGroups);
        }



        [HttpGet("{groupId}/users")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetGroupUsers([FromQuery(Name = "recursive")] bool recursive, int groupId)
        {
            var group = await context.Groups.Include(g => g.ChildGroups).Include(g => g.Users).FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            List<User> allUsers = new();
            
            if (recursive)
            {
                allUsers.AddRange(await GetRecursiveUsers(group, new List<User>()));
            }

            allUsers.AddRange(group.Users);

            return Ok(allUsers.Distinct());
        }

        
        
        private async Task<List<User>> GetRecursiveUsers(Group group, List<User> allUsers)
        {
            if (!group.ChildGroups.Any())
            {
                return allUsers;
            }

            foreach (var childId in group.ChildGroups.Select(g => g.Id))
            {
                var child =
                    await context.Groups
                        .Include(g => g.ChildGroups)
                        .Include(g => g.Users)
                        .FirstOrDefaultAsync(g => g.Id == childId);

                allUsers.AddRange(child.Users);
                allUsers = await GetRecursiveUsers(child, allUsers);
            }

            return allUsers;
        }
    }
}