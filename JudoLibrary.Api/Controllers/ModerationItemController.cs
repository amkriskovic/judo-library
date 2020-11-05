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