using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JudoLibrary.Api.BackgroundServices.VideoEditing;
using JudoLibrary.Api.Settings;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

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
        
        [HttpPut("me/image")]
        // IFormFile represents a file sent with the HttpRequest.
        public async Task<IActionResult> UpdateProfileImage(IFormFile image, [FromServices] IFileManager fileManager)
        {
            // If image is null -> 400
            if (image == null) return BadRequest();
            
            // Grab UserId from ApiController
            var userId = UserId;
            
            // Grab the User => compare User Id with userId that's coming from User Claim -> "sub"
            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));

            // If no user -> 404
            if (user == null) return NoContent();

            // Generate profile picture file name
            var fileName = JudoLibraryConstants.Files.GenerateProfileFileName();

            // Create a IO File => provide temporary save path based on fileName
            await using (var stream = System.IO.File.Create(fileManager.GetSavePath(fileName)))
            {
                // Input stream -> IFF -> image -> open the input stream -> write
                using (var imageProcessor = await Image.LoadAsync(image.OpenReadStream()))
                {
                    // Resize the given image in place and return it for chaining.
                    // 48x48
                    imageProcessor.Mutate(x => x.Resize(48, 48));

                    // Save based on stream that we created and image encoder
                    await imageProcessor.SaveAsync(stream, new JpegEncoder());
                } // Dispose - releasing memory into a memory pool ready for the next image you wish to process.
            } // Dispose


            // Assign fileName that we generated to Users Image fileName
            user.Image = fileManager.GetFileUrl(fileName, FileType.Image);

            await _ctx.SaveChangesAsync();

            return Ok(user);
        }
    }
}