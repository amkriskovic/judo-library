using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using JudoLibrary.Api.Form;
using JudoLibrary.Api.ViewModels;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.AspNetCore.Authorization;
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
        // TechniqueViewModels.Projection is responsible for giving us response
        [HttpGet]
        public IEnumerable<object> GetAllTechniques() => _context.Techniques.Select(TechniqueViewModels.Projection).ToList();
        
        // GET -> /api/techniques/protected
        [HttpGet("protected")]
        // Providing Policy to authorize 
        [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName)]
        public string GetProtectedAuth() => "protected API Resource";
        
        // GET -> /api/techniques/mod
        [HttpGet("mod")]
        // Providing our custom Policy constant to authorize as Mod
        [Authorize(Policy = JudoLibraryConstants.Policies.Mod)]
        public string GetModeratorAuth() => "moderator API Resource";
        
        // GET -> /api/techniques/{id}
        [HttpGet("{id}")]
        public object GetTechnique(string id) => _context.Techniques
            .Where(t => t.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase))
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
                // Create TechniqueForm -> Id | based on TechniqueForm -> Name | ' ' -> '-' ==> slug
                Id = techniqueForm.Name.Replace(" ", "-").ToLowerInvariant(),
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
            
            // Save
            _context.Add(technique);
            await _context.SaveChangesAsync();

            // Return created technique that we pass to TechniqueViewModels
            return TechniqueViewModels.Create(technique);
        }
        
        // PUT -> /api/techniques
        [HttpPut]
        public async Task<object> UpdateTechnique([FromBody] Technique technique)
        {
            // Check if TechniqueForm exists
            if (string.IsNullOrEmpty(technique.Id))
                return null;
            
            // Update
            _context.Add(technique);
            await _context.SaveChangesAsync();

            // Return created technique that we pass to TechniqueViewModels
            return TechniqueViewModels.Create(technique);
        }
        
        
        // DELETE -> /api/techniques/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTechnique(string id)
        {
            // Grab technique from DB
            var technique = _context.Techniques.FirstOrDefault(t => t.Id.Equals(id));
            
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