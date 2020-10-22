using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/techniques")]
    public class TechniquesController : ControllerBase
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
            .Select(TechniqueViewModels.Projection)
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
                .Select(TechniqueViewModels.Projection)
                .FirstOrDefault();

            if (technique == null) return NoContent();

            return Ok(technique);
        }

        // GET -> /api/techniques/{techniqueId}/submissions
        // Get all submissions for particular technique | Passing technique Id as param | Including videos for technique
        [HttpGet("{techniqueId}/submissions")]
        public object GetAllSubmissionsForTechnique(string techniqueId) => _context.Submissions
            .Include(s => s.Video)
            .Include(s => s.User)
            .Where(s => s.TechniqueId.Equals(techniqueId))
            .Select(SubmissionViewModels.Projection)
            .ToList();

        // POST -> /api/techniques
        // Create technique, sending json from the body of the request, TechniqueForm is responsible for creating technique
        [HttpPost]
        public async Task<object> CreateTechnique([FromBody] TechniqueForm techniqueForm)
        {
            // Create Technique, mapping props from trickForm to Technique
            var technique = new Technique
            {
                // Create TechniqueForm -> Slug | based on TechniqueForm -> Name | ' ' -> '-' ==> slug
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
                    .ToList()
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

            // Invoke delegate Create which is our ViewModel then return ViewModel
            return TechniqueViewModels.Create(technique);
        }

        // PUT -> /api/techniques
        [HttpPut]
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
                    // saId is collection of int's that's coming from form SetUpAttacks(IE<int>), then we are selecting that int which
                    // represents SetUpAttack and assigning it to SetUpAttackId 
                    .Select(saId => new TechniqueSetupAttack {SetUpAttackId = saId})
                    .ToList(),

                FollowUpAttacks = techniqueForm.FollowUpAttacks
                    .Select(faId => new TechniqueFollowupAttack {FollowUpAttackId = faId})
                    .ToList(),

                Counters = techniqueForm.Counters
                    .Select(cId => new TechniqueCounter {CounterId = cId})
                    .ToList()
            };

            // Add newTechnique to DB
            _context.Add(newTechnique);

            // Save newTechnique to DB
            await _context.SaveChangesAsync();

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