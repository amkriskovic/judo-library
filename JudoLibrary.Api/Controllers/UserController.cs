using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JudoLibrary.Api.Controllers
{
    [Route("api/users")]
    [Authorize(JudoLibraryConstants.Policies.User)] // Authorize whole Controller => you need be a User to access this
    public class UserController : ApiController
    {
        private readonly AppDbContext _ctx;

        public UserController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // Getting our particular User
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            // Grab the UserId from | User ->> part of ControllerBase
            var userId = UserId;

            // If there is no userId
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            // Grab the User => compare User Id with userId that's coming from User Claim -> "sub"
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));

            // If user is not equal to null -> return the User
            if (user != null)
                return Ok(user);

            // If User is null -> Instantiate new User
            user = new User
            {
                // Grab the userId and set it to Id
                Id = userId,

                // Grab the PreferredUserName claim type -> value from JWT, this claim should be contained in access_token
                Username = PreferredUsername
            };

            // Add the User to ctx
            _ctx.Add(user);

            // Save User to DB
            await _ctx.SaveChangesAsync();

            // Return User
            return Ok(user);
        }

        // Getting a particular User based on id
        [HttpGet("{id}")]
        public IActionResult GetUser(string id) => Ok();

        // Getting a Submissions for particular User based on User id
        [HttpGet("{id}/submissions")]
        public Task<List<Submission>> GetUserSubmissions(string id)
        {
            return _ctx.Submissions
                .Include(s => s.Video)
                .Where(s => s.UserId.Equals(id))
                .ToListAsync();
        }
    }
}