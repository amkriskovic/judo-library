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
            .Where(t => t.Active)
            .Select(TechniqueViewModels.Projection)
            .ToList();

        // GET -> /api/techniques/{id}
        [HttpGet("{id}")]
        public object GetTechnique(string id) => _context.Techniques
            .Where(t => t.Slug.Equals(id, StringComparison.InvariantCultureIgnoreCase) && t.Active)
            .Select(TechniqueViewModels.Projection)
            .FirstOrDefault();

        // GET -> /api/techniques/{techniqueId}/submissions
        // Get all submissions for particular technique | Passing technique Id as param | Including videos for technique
        [HttpGet("{techniqueId}/submissions")]
        public IEnumerable<Submission> GetAllSubmissionsForTechnique(string techniqueId) => _context.Submissions
            .Include(s => s.Video)
            .Where(s => s.TechniqueId.Equals(techniqueId))
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
                Version = 1,
                Name = techniqueForm.Name,
                Description = techniqueForm.Description,
                Category = techniqueForm.Category,
                SubCategory = techniqueForm.SubCategory,
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
                Id = technique.Id,
                Slug = technique.Slug,
                // Bump the new Technique Version to + 1 from original Technique that we trying to edit
                Version = technique.Version + 1,
                
                Name = techniqueForm.Name,
                Description = techniqueForm.Description,
                Category = techniqueForm.Category,
                SubCategory = techniqueForm.SubCategory,
                
                // Collections
                SetUpAttacks = techniqueForm.SetUpAttacks
                    // saId is collection of int's that's coming from form SetUpAttacks(IE<int>), then we are selecting that int which
                    // represents SetUpAttack and assigning it to SetUpAttackId 
                    .Select(saId => new TechniqueSetupAttack{SetUpAttackId = saId})
                    .ToList(),
                
                FollowUpAttacks = techniqueForm.FollowUpAttacks
                    .Select(faId => new TechniqueFollowupAttack{FollowUpAttackId = faId})
                    .ToList(),
                
                Counters = techniqueForm.Counters
                    .Select(cId => new TechniqueCounter{CounterId = cId})
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