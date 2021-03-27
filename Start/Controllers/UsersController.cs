using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using UserManagement.Data;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private UserManagementDataContext context;

        public UsersController(UserManagementDataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Group>> GetSignedInUser()
        {
            string nameIdentifier = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(await context.Users.FirstOrDefaultAsync(u => u.NameIdentifier == nameIdentifier));
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> GetAll([FromQuery(Name = "filter")] string? filter)
        {
            if (filter != null)
            {
                return Ok(context.Users
                    .Where(u => u.Email.Contains(filter)
                                || (u.FirstName != null && u.FirstName.Contains(filter))
                                || (u.LastName != null && u.LastName.Contains(filter))));
            }

            return Ok(await context.Users.ToListAsync());
        }
    }
}