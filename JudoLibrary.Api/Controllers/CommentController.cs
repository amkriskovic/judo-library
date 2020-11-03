using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JudoLibrary.Api.Form;
using JudoLibrary.Api.ViewModels;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    // Class for creating replies to comments
    [Route("/api/comments")]
    [Authorize(JudoLibraryConstants.Policies.User)]
    public class CommentController : ApiController
    {
        private readonly AppDbContext _ctx;

        public CommentController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // GET -> /api/comments/{id}/replies
        // Get replies for comment => parent comment =>> {id} is base comment
        [HttpGet("{id}/replies")]
        public IEnumerable<object> GetRepliesForComment(int id) =>
            _ctx.Comments
                // Where parentId which is -> replies parent comment id === base comment id
                .Where(c => c.ParentId.Equals(id))
                .Select(CommentViewModel.Projection)
                .ToList();

        // POST -> /api/comments
        [HttpPost]
        public async Task<IActionResult> CreateComment(
            [FromBody] CommentForm commentForm,
            [FromServices] CommentCreationContext commentCreationContext)
        {
            // Provide UserId when creating comment, so we know who it belongs to
            var comment = await commentCreationContext
                .Setup(UserId)
                .CreateCommentAsync(commentForm);

            // Return ok with created comment
            return Ok(CommentViewModel.Create(comment));
        }
    }
}