using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        // Getting our particular User
        [HttpGet("me")]
        public IActionResult GetMe() => Ok();

        // Getting a particular User based on id
        [HttpGet("{id}")]
        public IActionResult GetUser(string id) => Ok();
    }
}