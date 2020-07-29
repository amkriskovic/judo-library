using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JudoLibrary.Api.Form;
using JudoLibrary.Api.ViewModels;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.AspNetCore.Mvc;

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
        // TechniqueViewModels.Default is responsible for giving us response
        [HttpGet]
        public IEnumerable<object> GetAllTechniques() => _context.Techniques.Select(TechniqueViewModels.Default).ToList();
        
        // GET -> /api/techniques/{id}
        [HttpGet("{id}")]
        public object GetTechnique(string id) => _context.Techniques
            .Where(t => t.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase))
            .Select(TechniqueViewModels.Default)
            .FirstOrDefault();

        // GET -> /api/techniques/{id}/submissions
        // Get all submissions for particular technique | Passing technique Id as param
        [HttpGet("{id}/submissions")]
        public IEnumerable<Submission> GetAllSubmissionsForTechnique(string techniqueId) => _context.Submissions
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
                Category = techniqueForm.CategoryId,
                SubCategory = techniqueForm.SubCategoryId,
            };
            
            // Save
            _context.Add(technique);
            await _context.SaveChangesAsync();

            // Compile and Invoke Expression which is our ViewModel then return ViewModel
            return TechniqueViewModels.Default.Compile().Invoke(technique);
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

            // Compile and Invoke Expression which is our ViewModel then return ViewModel
            return TechniqueViewModels.Default.Compile().Invoke(technique);
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