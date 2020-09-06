using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JudoLibrary.Api.ViewModels;
using JudoLibrary.Data;
using JudoLibrary.Models;
using JudoLibrary.Models.Moderation;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
    [Route("/api/moderation-items")]
    public class ModerationItemController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public ModerationItemController(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        // GET -> /api/moderation-items
        [HttpGet]
        public IEnumerable<ModerationItem> GetModerationItems() => _ctx.ModerationItems.ToList();
        
        // GET -> /api/moderation-items/{id}
        [HttpGet("{id}")]
        public ModerationItem GetModerationItem(int id) => _ctx.ModerationItems.FirstOrDefault(mi => mi.Id.Equals(id));
        
        // GET -> /api/moderation-items/{id}/comments
        // Listing comments for particular moderation item(id)
        [HttpGet("{id}/comments")]
        public IEnumerable<object> GetCommentsForModerationItem(int id) =>
            _ctx.Comments
                // Where moderation item id for comment is equal to id that is passed
                .Where(c => c.ModerationItemId.Equals(id))
                .Select(CommentViewModel.Projection)
                .ToList();
        
        // POST -> /api/moderation-items/{id}/comments
        // Create comment for particular moderation modItem
        // Passing id from url, and from body -> comment
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> CreateCommentForModerationItem(int id, [FromBody] Comment comment)
        {
            // If moderationItem doesnt exist -> we dont have anything to moderate
            if (!_ctx.ModerationItems.Any(mi => mi.Id.Equals(id)))
                return NoContent();
            
            // First group starts with 2nd @ symbol, \B -> non word boundary => means as soon it tied to some word without space, it
            // will ignore it. Allowing lower & upper case characters, numbers, dash and underscore, + -> one or more from collection
            // that is specified in ([]), ?<tag> is used to name group => (), used as reference to a group when we want to find that
            // particular regex group
            var regex = new Regex(@"\B(?<tag>@[a-zA-Z0-9-_]+)");

            // Assign original comment content that's being processed via LINQ, to Html comment content
            comment.HtmlContent = regex
                // This is what we want to match -> plain comment
                .Matches(comment.Content)
                // We start with comment.Content as initial value, 2nd param is func that takes string, which is original comment content
                // and match parameter which gives us Match options
                .Aggregate(comment.Content, (content, match) =>
                {
                    // Extract from regex <tag> group and grab the value => string
                    var tag = match.Groups["tag"].Value;

                    // Replacing matched regex tag which is a string, with link, so we have navigation, taggable/linkable ?user?
                    // And return that as content, which becomes link
                    return content.Replace(tag, $"<a href=\"{tag}-user-link\">{tag}</a>");
                });

            
            // Assign id from moderation item that we moderating to comment's moderation item id
            comment.ModerationItemId = id;
            
            // Add comment to particular moderation modItem that we found by id, to list of comments
            _ctx.Add(comment);
            
            // Save changes to DB
            await _ctx.SaveChangesAsync();
            
            // Return Ok response with created comment
            return Ok(CommentViewModel.Create(comment));
        }
        
        // GET -> /api/moderation-items/{id}/reviews
        // Listing reviews for particular moderation item(id)
        [HttpGet("{id}/reviews")]
        public IEnumerable<Review> GetReviewsForModerationItem(int id) =>
            _ctx.Reviews
                // Where moderation item id for review is equal to id that is passed
                .Where(r => r.ModerationItemId.Equals(id))
                .ToList();
        
        // POST -> /api/moderation-items/{id}/reviews
        // Create review for particular moderation modItem
        // Passing id from url, and from body -> review 
        [HttpPost("{id}/reviews")]
        public async Task<IActionResult> CreateReviewForModerationItem(int id, [FromBody] Review review)
        {
            // If moderationItem doesnt exist -> we dont have anything to moderate
            if (!_ctx.ModerationItems.Any(mi => mi.Id.Equals(id)))
                return NoContent();

            // Assign id from moderation item that we moderating to review's moderation item id
            review.ModerationItemId = id;
            
            // Add review to particular moderation modItem that we found by id, to list of reviews
            _ctx.Add(review);
            
            // Save changes to DB
            await _ctx.SaveChangesAsync();
            
            // Return Ok response with created review
            return Ok(review);
        }
    }
}