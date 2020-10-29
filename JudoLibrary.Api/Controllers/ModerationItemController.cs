using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JudoLibrary.Api.Form;
using JudoLibrary.Api.ViewModels;
using JudoLibrary.Data;
using JudoLibrary.Models;
using JudoLibrary.Models.Moderation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IEnumerable<ModerationItem> GetModerationItems() => _ctx.ModerationItems
            .Where(mi => !mi.Deleted)
            .ToList();
        
        // GET -> /api/moderation-items/{id}
        [HttpGet("{id}")]
        public object GetModerationItem(int id) => _ctx.ModerationItems
            .Include(mi => mi.Comments)
            .Include(mi => mi.Reviews)
            .Where(mi => mi.Id == id)
            .Select(ModerationItemViewModels.Projection)
            .FirstOrDefault();
        
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
        // Created comment for particular moderation modItem
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
        // Created review for particular moderation modItem
        // Passing id from url, and from body -> review 
        [HttpPost("{id}/reviews")]
        public async Task<IActionResult> CreateReviewForModerationItem(
            int id, 
            [FromBody] ReviewForm reviewForm,
            [FromServices] VersionMigrationContext versionMigrationContext)
        {
            // Get the moderation item by comparing Id's -> include Reviews with it
            var modItem = _ctx.ModerationItems
                .Include(mi => mi.Reviews)
                .FirstOrDefault(mi => mi.Id == id);
                
            // If moderationItem doesnt exist -> we dont have anything to moderate -> 204
            if (modItem == null)
                return NoContent();
            
            // If moderation item is deleted -> 400
            if (modItem.Deleted)
                return BadRequest("Moderation item no longer exists!");
            
            // Created new Review for this particular MI
            var review = new Review
            {
                // Assign moderation-item - {id} (That's passed in) to Review's ModerationItemId
                ModerationItemId = id,
                Status = reviewForm.Status,
                Comment = reviewForm.Comment
            };

            // Add review to moderation item list of reviews
            _ctx.Add(review);

            try
            {
                // If moderation item Reviews count is greater or equal to 3 -> approved/rejected/pending
                if (modItem.Reviews.Count >= 3)
                {
                    // Passing ModerationItem to Migrate process
                    versionMigrationContext.Migrate(modItem);
                    
                    // "Delete" after migration
                    modItem.Deleted = true;
                }
                
                // Save changes to DB
                await _ctx.SaveChangesAsync();
            }
            catch (VersionMigrationContext.InvalidVersionException e)
            {
                return BadRequest(e.Message);
            }

            // Return Ok response with created review
            return Ok(ReviewViewModel.Create(review));
        }
    }
}