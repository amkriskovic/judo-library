using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JudoLibrary.Api.Form;
using JudoLibrary.Api.ViewModels;
using JudoLibrary.Data;
using JudoLibrary.Models.Moderation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JudoLibrary.Api.Controllers
{
    [Route("/api/moderation-items")]
    public class ModerationItemController : ApiController
    {
        private readonly AppDbContext _ctx;

        public ModerationItemController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // GET -> /api/moderation-items
        [HttpGet]
        public object GetAllModerationItems([FromQuery] FeedQuery feedQuery)
        {
            var moderationItems = _ctx.ModerationItems
                .Include(mi => mi.User)
                .Include(x => x.Reviews)
                .Where(x => !x.Deleted)
                .OrderFeed(feedQuery)
                .ToList();

            var targetMapping = new Dictionary<int, object>();
            foreach (var group in moderationItems.GroupBy(x => x.Type))
            {
                var targetIds = group.Select(m => m.Target).ToArray();

                if (group.Key == ModerationTypes.Technique)
                {
                    _ctx.Techniques
                        .Where(t => targetIds.Contains(t.Id))
                        .ToList()
                        .ForEach(technique => targetMapping[technique.Id] = TechniqueViewModels.CreateFlat(technique));
                }
            }

            return moderationItems.Select(x => new
            {
                x.Id, // Crucial to editing
                x.Current,
                x.Target,
                x.Reason,
                x.Type,

                Updated = x.Updated.ToLocalTime().ToString("HH:mm dd/MM/yyyy"),
                Reviews = x.Reviews.Select(r => r.Status).ToList(),
                User = UserViewModel.CreateFlat(x.User),
                TargetObject = targetMapping[x.Target],
            });
        }

        // GET -> /api/moderation-items/{id}
        [HttpGet("{id}")]
        public object GetModerationItem(int id) => _ctx.ModerationItems
            .Where(mi => mi.Id.Equals(id))
            .Select(ModerationItemViewModels.Projection)
            .FirstOrDefault();

        // GET -> /api/moderation-items/{id}/reviews
        // Listing reviews for particular moderation item(id)
        [HttpGet("{id}/reviews")]
        public IEnumerable<object> GetReviewsForModerationItem(int id) =>
            _ctx.Reviews
                .Include(x => x.User)
                .Where(r => r.ModerationItemId.Equals(id))
                .Select(ReviewViewModels.WithUserProjection)
                .ToList();

        // POST -> /api/moderation-items/{id}/reviews
        // Created review for particular moderation modItem
        // Passing id from url, and from body -> review 
        [HttpPut("{id}/reviews")]
        [Authorize(JudoLibraryConstants.Policies.Mod)]
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

            var review = _ctx.Reviews.FirstOrDefault(x => x.ModerationItemId == id && x.UserId == UserId);

            if (review == null)
            {
                // Created new Review for this particular MI
                review = new Review
                {
                    // Assign moderation-item - {id} (That's passed in) to Review's ModerationItemId
                    ModerationItemId = id,
                    Status = reviewForm.Status,
                    Comment = reviewForm.Comment,
                    UserId = UserId
                };

                // Add review to moderation item list of reviews
                _ctx.Add(review);
            }
            else
            {
                review.Comment = reviewForm.Comment;
                review.Status = reviewForm.Status;
            }

            try
            {
                int goal = 3, score = 9, wait = 0;

                foreach (var modItemReview in modItem.Reviews)
                {
                    if (modItemReview.Status == ReviewStatus.Approved)
                        score++;
                    
                    else if (modItemReview.Status == ReviewStatus.Rejected)
                        score--;
                    
                    else if (modItemReview.Status == ReviewStatus.Waiting)
                        wait++;
                }
                
                if (score >= goal + wait)
                {
                    // Passing ModerationItem to Migrate process
                    versionMigrationContext.Migrate(modItem);

                    // "Delete" after migration
                    modItem.Deleted = true;
                } else if (score <= -goal - wait)
                {
                    modItem.Deleted = true;
                    modItem.Rejected = true;
                }

                modItem.Updated = DateTime.UtcNow;

                // Save changes to DB
                await _ctx.SaveChangesAsync();
            }
            catch (VersionMigrationContext.InvalidVersionException e)
            {
                return BadRequest(e.Message);
            }

            // Return Ok response with created review
            return Ok();
        }
    }
}