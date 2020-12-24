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
        public object GetAllModerationItems([FromQuery] FeedQuery feedQuery, int user)
        {
            var query = _ctx.ModerationItems.Where(x => !x.Deleted);

            if (user == 1)
                query = query.Where(x => x.UserId == UserId);
            
            var moderationItems = query
                .Include(mi => mi.User)
                .Include(x => x.Reviews)
                .OrderFeed(feedQuery)
                .ToList();

            var targetMapping = new Dictionary<string, object>();
            foreach (var group in moderationItems.GroupBy(x => x.Type))
            {
                var targetIds = group
                    .Select(m => new[] {m.Target, m.Current})
                    .SelectMany(x => x)
                    .Where(x => x > 0)
                    .ToArray();

                if (group.Key == ModerationTypes.Technique)
                {
                    _ctx.Techniques
                        .Where(t => targetIds.Contains(t.Id))
                        .ToList()
                        .ForEach(technique => targetMapping[ModerationTypes.Technique + technique.Id] = TechniqueViewModels.CreateFlat(technique));
                }
                else if (group.Key == ModerationTypes.Category)
                {
                    _ctx.Categories
                        .Where(c => targetIds.Contains(c.Id))
                        .ToList()
                        .ForEach(category => targetMapping[ModerationTypes.Category + category.Id] = 
                            CategoryViewModels.CreateFlat(category));
                }
                else if (group.Key == ModerationTypes.SubCategory)
                {
                    _ctx.SubCategories
                        .Where(sc => targetIds.Contains(sc.Id))
                        .ToList()
                        .ForEach(subcategory => targetMapping[ModerationTypes.SubCategory + subcategory.Id] = 
                            SubCategoryViewModels.CreateFlat(subcategory));
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
                CurrentObject = x.Current > 0 ? targetMapping[x.Type + x.Current] : null,
                TargetObject = x.Target > 0 ? targetMapping[x.Type + x.Target] : null,
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