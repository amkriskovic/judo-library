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
            .Where(t => !t.Deleted && t.State == VersionState.Live)
            .Include(t => t.SetUpAttacks)
            .Include(t => t.FollowUpAttacks)
            .Include(t => t.Counters)
            .Include(t => t.User)
            .Select(TechniqueViewModels.Projection)
            .ToList();

        // GET -> /api/techniques/{value}
        [HttpGet("{value}")]
        public IActionResult GetTechnique(string value)
        {
            var technique = _context.Techniques
                .WhereIdOrSlug(value)
                .Include(t => t.TechniqueCategories)
                .Include(t => t.SetUpAttacks)
                .Include(t => t.FollowUpAttacks)
                .Include(t => t.Counters)
                .Include(t => t.User)
                .Select(TechniqueViewModels.FullProjection)
                .FirstOrDefault();

            if (technique == null) return NoContent();

            return Ok(technique);
        }
        
        // GET -> /api/techniques/{slug}/history
        [HttpGet("{slug}/history")]
        public IEnumerable<object> GetTechniqueHistory(string slug)
        {
            return _context.Techniques
                .Where(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && x.State != VersionState.Staged)
                .Include(t => t.TechniqueCategories)
                .Include(t => t.SetUpAttacks)
                .Include(t => t.FollowUpAttacks)
                .Include(t => t.Counters)
                .Include(t => t.User)
                .Select(TechniqueViewModels.FullProjection)
                .ToList();
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
        public async Task<IActionResult> CreateTechnique([FromBody] CreateTechniqueForm createTechniqueForm)
        {
            // Created Technique, mapping props from trickForm to Technique
            var technique = new Technique
            {
                // Created TechniqueForm -> Slug | based on TechniqueForm -> Name | ' ' -> '-' ==> slug
                Slug = createTechniqueForm.Name.Replace(" ", "-").ToLowerInvariant(),
                Name = createTechniqueForm.Name,
                Version = 1,

                Description = createTechniqueForm.Description,
                TechniqueCategories = new List<TechniqueCategory>
                {
                    new TechniqueCategory {CategoryId = createTechniqueForm.Category}
                },
                TechniqueSubCategories = new List<TechniqueSubCategory>
                {
                    new TechniqueSubCategory {SubCategoryId = createTechniqueForm.SubCategory}
                },

                // Collections
                SetUpAttacks = createTechniqueForm.SetUpAttacks
                    // setUpAttackId is pulling values from TechniqueSetupAttack table -> SetUpAttackId prop, and we are
                    // Selecting that and assigning to SetUpAttacks
                    // * Assigning to SetUpAttackId value of setUpAttackId that came from [FromBody] when filling form.
                    // * SetUpAttacks hold value/s then we are selecting them and as above described, mapping them to SetUpAttackId so EF knows what is what
                    .Select(setUpAttackId => new TechniqueSetupAttack {SetUpAttackId = setUpAttackId})
                    .ToList(),
                FollowUpAttacks = createTechniqueForm.FollowUpAttacks
                    .Select(followUpAttackId => new TechniqueFollowupAttack {FollowUpAttackId = followUpAttackId})
                    .ToList(),
                Counters = createTechniqueForm.Counters
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
                Type = ModerationTypes.Technique,
                UserId = UserId
            });

            // Save ModerationItem to DB
            await _context.SaveChangesAsync();

            // Invoke delegate Created which is our ViewModel then return ViewModel
            return Ok();
        }

        // PUT -> /api/techniques
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateTechnique([FromBody] UpdateTechniqueForm createTechniqueForm)
        {
            // Extract existing technique from DB by comparing Id(int)
            var technique = _context.Techniques.FirstOrDefault(t => t.Id == createTechniqueForm.Id);

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

                Description = createTechniqueForm.Description,
                TechniqueCategories = new List<TechniqueCategory>
                {
                    new TechniqueCategory {CategoryId = createTechniqueForm.Category}
                },
                TechniqueSubCategories = new List<TechniqueSubCategory>
                {
                    new TechniqueSubCategory {SubCategoryId = createTechniqueForm.SubCategory}
                },

                // Collections
                SetUpAttacks = createTechniqueForm.SetUpAttacks
                    // setUpAttackId is pulling values from TechniqueSetupAttack table -> SetUpAttackId prop, and we are
                    // Selecting that and assigning to SetUpAttacks
                    // * Assigning to SetUpAttackId value of setUpAttackId that came from [FromBody] when filling form.
                    // * SetUpAttacks hold value/s then we are selecting them and as above described, mapping them to SetUpAttackId so EF knows what is what
                    .Select(setUpAttackId => new TechniqueSetupAttack {SetUpAttackId = setUpAttackId})
                    .ToList(),
                FollowUpAttacks = createTechniqueForm.FollowUpAttacks
                    .Select(followUpAttackId => new TechniqueFollowupAttack {FollowUpAttackId = followUpAttackId})
                    .ToList(),
                Counters = createTechniqueForm.Counters
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

                Type = ModerationTypes.Technique,
                
                Reason = createTechniqueForm.Reason,
                UserId = UserId
            });

            // Save MI to DB
            await _context.SaveChangesAsync();

            // Return created technique that we pass to TechniqueViewModels
            return Ok();
        }
        
        [HttpPut("staged")]
        [Authorize]
        public async Task<IActionResult> UpdateStaged([FromBody] UpdateStagedTechniqueForm form)
        {
            var technique = _context.Techniques
                .Include(x => x.TechniqueCategories)
                .Include(x => x.TechniqueSubCategories)
                .Include(x => x.SetUpAttacks)
                .Include(x => x.FollowUpAttacks)
                .Include(x => x.Counters)
                .FirstOrDefault(x => x.Id == form.Id);

            if (technique == null) return NoContent();
            if (technique.UserId != UserId) return BadRequest("Can't edit this technique.");

            technique.Description = form.Description;
            
            technique.TechniqueCategories = new List<TechniqueCategory> {new TechniqueCategory {CategoryId = form.Category}};
            technique.TechniqueSubCategories = new List<TechniqueSubCategory> {new TechniqueSubCategory {SubCategoryId = form.SubCategory}};

            technique.SetUpAttacks = form.SetUpAttacks
                .Select(x => new TechniqueSetupAttack {SetUpAttackId = x})
                .ToList();
            technique.FollowUpAttacks = form.FollowUpAttacks
                .Select(x => new TechniqueFollowupAttack {FollowUpAttackId = x})
                .ToList();
            technique.Counters = form.Counters
                .Select(x => new TechniqueCounter {CounterId = x})
                .ToList();

            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}