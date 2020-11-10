using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        // GET -> /api/{parentId}/{parentType}
        [HttpGet("{parentId}/{parentType}")]
        public IEnumerable<object> GetRepliesForComment(int parentId,
            CommentCreationContext.ParentType parentType,
            [FromQuery] FeedQuery feedQuery)
        {
            Expression<Func<Comment, bool>> filter = parentType switch
            {
                CommentCreationContext.ParentType.ModerationItem => comment => comment.ModerationItemId == parentId,
                CommentCreationContext.ParentType.Submission => comment => comment.SubmissionId == parentId,
                CommentCreationContext.ParentType.Comment => comment => comment.ParentId == parentId,
                _ => throw new ArgumentException()
            };

            return _ctx.Comments
                .Where(filter)
                .OrderFeed(feedQuery)
                .Select(CommentViewModel.Projection)
                .ToList();
        }

        // POST -> /api/comments
        [HttpPost]
        public async Task<IActionResult> CreateComment(
            [FromBody] CommentForm commentForm,
            [FromServices] CommentCreationContext commentCreationContext)
        {
            try
            {
                // Provide UserId when creating comment, so we know who it belongs to
                var comment = await commentCreationContext
                    .Setup(UserId)
                    .CreateCommentAsync(commentForm);

                // Return ok with created comment
                return Ok(CommentViewModel.Create(comment));
            }
            catch (CommentCreationContext.ParentNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}