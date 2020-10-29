using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JudoLibrary.Api.ViewModels;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    // Class for creating replies to comments
    [ApiController]
    [Route("/api/comments")]
    public class CommentController : ControllerBase
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

        // POST -> /api/comments/{id}/replies
        // Created reply for particular comment => base comment =>> {id} is base comment
        [HttpPost("{id}/replies")]
        public async Task<IActionResult> CreateReplyForComment(int id, [FromBody] Comment reply)
        {
            // Grab the actual comment => base comment
            var baseComment = _ctx.Comments.FirstOrDefault(c => c.Id.Equals(id));
            
            // If baseComment doesnt exist -> we cant create reply for it
            if (baseComment == null)
            {
                // return no content
                return NoContent();
            }
            
            // Created regex
            var regex = new Regex(@"\B(?<tag>@[a-zA-Z0-9-_]+)");

            // Assign original reply content that's being processed via LINQ, to Html reply content
            reply.HtmlContent = regex
                .Matches(reply.Content)
                .Aggregate(reply.Content, (content, match) =>
                {
                    // Extract from regex <tag> group and grab the value => string
                    var tag = match.Groups["tag"].Value;

                    return content.Replace(tag, $"<a href=\"{tag}-user-link\">{tag}</a>");
                });
            
            // Add reply to comment list of replies
            baseComment.Replies.Add(reply);
            
            // Save changes to DB -> this will atomatically populate parentId for reply
            await _ctx.SaveChangesAsync();

            // Return ok with created reply
            return Ok(CommentViewModel.Create(reply));
        }
    }
}