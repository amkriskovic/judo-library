using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JudoLibrary.Api.Form;
using JudoLibrary.Api.ViewModels;
using JudoLibrary.Data;
using JudoLibrary.Models;
using JudoLibrary.Models.Moderation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JudoLibrary.Api.Controllers
{
    [Route("api/techniques")]
    public class TechniquesController : ApiController
    {
        private readonly AppDbContext _context;

        public TechniquesController(AppDbContext context)
        {
            _context = context;
        }

        // GET -> /api/techniques
        // TechniqueViewModels.Projection is responsible for giving us response => grab all active Techniques
        [HttpGet]
        public IEnumerable<object> GetAllTechniques() => _context.Techniques
            .AsNoTracking()
            .Where(t => t.Active)
            .Include(t => t.SetUpAttacks)
            .Include(t => t.FollowUpAttacks)
            .Include(t => t.Counters)
            .Include(t => t.User)
            .Select(TechniqueViewModels.UserProjection)
            .ToList();

        // GET -> /api/techniques/{id}
        [HttpGet("{id}")]
        public IActionResult GetTechnique(string id)
        {
            // Make an Query -> DbSet<Techniques>
            var query = _context.Techniques.AsQueryable();

            // Try to parse id as int => means _modId is calling it => /moderation => current & target
            if (int.TryParse(id, out var intId))
            {
                // Technique Id is an Int32
                query = query.Where(t => t.Id == intId);
            }
            else
            {
                // Technique Id is an string => grab only active ones => compare the slug with id => /index/techniqueSlug
                query = query.Where(t => t.Slug.Equals(id, StringComparison.CurrentCultureIgnoreCase) && t.Active);
            }

            // Get the technique from query
            var technique = query
                .Include(t => t.SetUpAttacks)
                .Include(t => t.FollowUpAttacks)
                .Include(t => t.Counters)
                .Include(t => t.User)
                .Select(TechniqueViewModels.FullProjection)
                .FirstOrDefault();

            if (technique == null) return NoContent();

            return Ok(technique);
        }

        // GET -> /api/techniques/{techniqueId}/submissions
        // Get all submissions for particular technique | Passing technique Id as param | Including videos for technique
        [HttpGet("{techniqueId}/submissions")]
        public IEnumerable<object> GetAllSubmissionsForTechnique(string techniqueId, [FromQuery] FeedQuery feedQuery)
        {
            return _context.Submissions
                .Include(s => s.Video)
                .Include(s => s.User)
                .Where(s => s.TechniqueId.Equals(techniqueId, StringComparison.InvariantCultureIgnoreCase))
                .OrderFeed(feedQuery)
                .Select(SubmissionViewModels.PerspectiveProjection(UserId))
                .ToList();
        }
        
        // POST -> /api/techniques
        // Created technique, sending json from the body of the request, TechniqueForm is responsible for creating technique
        [HttpPost]
        [Authorize]
        public async Task<object> CreateTechnique([FromBody] TechniqueForm techniqueForm)
        {
            // Created Technique, mapping props from trickForm to Technique
            var technique = new Technique
            {
                // Created TechniqueForm -> Slug | based on TechniqueForm -> Name | ' ' -> '-' ==> slug
                Slug = techniqueForm.Name.Replace(" ", "-").ToLowerInvariant(),
                Name = techniqueForm.Name,
                Version = 1,

                Description = techniqueForm.Description,
                Category = techniqueForm.Category,
                SubCategory = techniqueForm.SubCategory,

                // Collections
                SetUpAttacks = techniqueForm.SetUpAttacks
                    // setUpAttackId is pulling values from TechniqueSetupAttack table -> SetUpAttackId prop, and we are
                    // Selecting that and assigning to SetUpAttacks
                    // * Assigning to SetUpAttackId value of setUpAttackId that came from [FromBody] when filling form.
                    // * SetUpAttacks hold value/s then we are selecting them and as above described, mapping them to SetUpAttackId so EF knows what is what
                    .Select(setUpAttackId => new TechniqueSetupAttack {SetUpAttackId = setUpAttackId})
                    .ToList(),
                FollowUpAttacks = techniqueForm.FollowUpAttacks
                    .Select(followUpAttackId => new TechniqueFollowupAttack {FollowUpAttackId = followUpAttackId})
                    .ToList(),
                Counters = techniqueForm.Counters
                    .Select(counterId => new TechniqueCounter {CounterId = counterId})
                    .ToList(),

                // UserId is coming from ApiController -> JwtClaimTypes.Subject
                UserId = UserId
            };

            // Add technique to DB
            _context.Add(technique);

            // Save Technique to DB
            await _context.SaveChangesAsync();

            // We need to add particular Technique to ModerationItem first to be approved/rejected/pending -> after that it can be visible at index page
            // Specifying Target which is technique's Id and Type which is Technique => that we created above
            _context.Add(new ModerationItem
            {
                Target = technique.Id, // This is where Id is going to be generated
                Type = ModerationTypes.Technique
            });

            // Save ModerationItem to DB
            await _context.SaveChangesAsync();

            // Invoke delegate Created which is our ViewModel then return ViewModel
            return TechniqueViewModels.Create(technique);
        }

        // PUT -> /api/techniques
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateTechnique([FromBody] TechniqueForm techniqueForm)
        {
            // Extract existing technique from DB by comparing Id(int)
            var technique = _context.Techniques.FirstOrDefault(t => t.Id == techniqueForm.Id);

            // If technique is null -> NoContent 204
            if (technique == null) return NoContent();

            // When we are updating the technique we are NOT modifying existing one, we are creating New one
            var newTechnique = new Technique
            {
                // Those 3 will come from original Technique -> NOT modifying -> Slug depends on the Name
                Slug = technique.Slug,
                Name = technique.Name,
                // Bump the new Technique Version to + 1 from original Technique that we trying to edit
                Version = technique.Version + 1,

                Description = techniqueForm.Description,
                Category = techniqueForm.Category,
                SubCategory = techniqueForm.SubCategory,

                // Collections
                SetUpAttacks = techniqueForm.SetUpAttacks
                    // setUpAttackId is pulling values from TechniqueSetupAttack table -> SetUpAttackId prop, and we are
                    // Selecting that and assigning to SetUpAttacks
                    // * Assigning to SetUpAttackId value of setUpAttackId that came from [FromBody] when filling form.
                    // * SetUpAttacks hold value/s then we are selecting them and as above described, mapping them to SetUpAttackId so EF knows what is what
                    .Select(setUpAttackId => new TechniqueSetupAttack {SetUpAttackId = setUpAttackId})
                    .ToList(),
                FollowUpAttacks = techniqueForm.FollowUpAttacks
                    .Select(followUpAttackId => new TechniqueFollowupAttack {FollowUpAttackId = followUpAttackId})
                    .ToList(),
                Counters = techniqueForm.Counters
                    .Select(counterId => new TechniqueCounter {CounterId = counterId})
                    .ToList(),

                // UserId is coming from ApiController -> JwtClaimTypes.Subject
                UserId = UserId
            };

            // Add newTechnique to DB
            _context.Add(newTechnique);

            // Save newTechnique to DB
            await _context.SaveChangesAsync();

            // Iterate over TechniqueSetupAttacks where SetUpAttackId which points to specific Technique is equal to Technique Id
            var techniqueSetupAttacks = _context.TechniqueSetupAttacks.Where(x => x.SetUpAttackId == technique.Id);
            var techniqueFollowupAttacks = _context.TechniqueFollowupAttacks.Where(x => x.FollowUpAttackId == technique.Id);
            var techniqueCounters = _context.TechniqueCounters.Where(x => x.CounterId == technique.Id);
            
            // Loop over those techniqueSetupAttacks
            foreach (var techniqueSetupAttack in techniqueSetupAttacks)
            {
                // Need to separately update TechniqueSetupAttack, so that SetUpAttackId points to newTechniques id
                // Add them to DB, but with updated Technique Id (new technique that we edited)
                _context.Add(new TechniqueSetupAttack
                    {TechniqueId = techniqueSetupAttack.TechniqueId, SetUpAttackId = newTechnique.Id});
            }
            
            foreach (var techniqueFollowupAttack in techniqueFollowupAttacks)
            {
                _context.Add(new TechniqueFollowupAttack
                    {TechniqueId = techniqueFollowupAttack.TechniqueId, FollowUpAttackId = newTechnique.Id});
            }

            foreach (var techniqueCounter in techniqueCounters)
            {
                _context.Add(new TechniqueCounter
                    {TechniqueId = techniqueCounter.TechniqueId, CounterId = newTechnique.Id});
            }

            // Adding this newTechnique to MI -> First need to approve it to be visible
            _context.Add(new ModerationItem
            {
                // Using Id's for versioning
                // Grab technique Id and assign it to Current (version)
                Current = technique.Id,

                // Assign Id from newly added/created technique to Target(version)
                Target = newTechnique.Id,

                Type = ModerationTypes.Technique
            });

            // Save MI to DB
            await _context.SaveChangesAsync();

            // Return created technique that we pass to TechniqueViewModels
            return Ok(TechniqueViewModels.Create(newTechnique));
        }


        // DELETE -> /api/techniques/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTechnique(string id)
        {
            // Grab technique from DB by comparing Slugs
            var technique = _context.Techniques.FirstOrDefault(t => t.Slug == id);

            // If technique not exists
            if (technique == null)
                return NotFound();

            // Mark as deleted
            technique.Deleted = true;

            await _context.SaveChangesAsync();

            return Ok();
        }
        
    }
}