using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JudoLibrary.Api.Form;
using JudoLibrary.Api.ViewModels;
using JudoLibrary.Data;
using JudoLibrary.Data.VersionMigrations;
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
                else if (group.Key == ModerationTypes.Category)
                {
                    _ctx.Categories
                        .Where(t => targetIds.Contains(t.Id))
                        .ToList()
                        .ForEach(category => targetMapping[category.Id] = 
                            CategoryViewModels.CreateFlat(category));
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
            [FromBody] ModerationItemReviewContext.ReviewForm reviewForm,
            [FromServices] ModerationItemReviewContext moderationItemReviewContext)
        {
            try
            {
                await moderationItemReviewContext.Review(id, UserId, reviewForm);
            }
            catch (VersionMigrationContext.InvalidVersionException e)
            {
                return BadRequest(e.Message);
            }
            catch (ModerationItemReviewContext.ModerationItemNotFound)
            {
                return NoContent();
            }

            return Ok();
        }
    }
}