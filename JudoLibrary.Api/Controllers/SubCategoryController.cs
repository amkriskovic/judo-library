using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JudoLibrary.Api.ViewModels;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
    [Route("/api/subcategories")]
    public class SubCategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubCategoryController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET -> /api/subcategories
        [HttpGet]
        public IEnumerable<object> GetAllSubCategories() => _context.SubCategories
            .Select(SubCategoryViewModels.Projection)
            .ToList();
        
        // GET -> /api/subcategories/{id}
        [HttpGet("{id}")]
        public SubCategory GetSubCategory(string id) => _context.SubCategories
            .FirstOrDefault(c => c.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        
        // GET -> /api/subcategories/{id}/techniques
        // Get all techniques for particular sub-category | Passing sub-category Id as param
        [HttpGet("{id}/techniques")]
        public IEnumerable<Technique> GetAllTechniquesForSubCategory(string id) => _context.Techniques
            .Where(t => t.SubCategory.Equals(id))
            .ToList();
        
        // POST -> /api/subcategories
        // Created category, sending json from the body of the request
        [HttpPost]
        public async Task<SubCategory> CreateSubCategory([FromBody] SubCategory subCategory)
        {
            // Created SubCategory -> Id | based on SubCategory -> Name | ' ' -> '-' ==> slug
            subCategory.Id = subCategory.Name.Replace(" ", "-").ToLowerInvariant();
            
            // Add SubCategory to DB
            _context.Add(subCategory);
            
            // Saves changes async so it doesn't wait for actual saving time to DB, await prevents blocking UI
            await _context.SaveChangesAsync();

            return subCategory;
        }
        
        // PUT -> /api/subcategories
        [HttpPut]
        public async Task<SubCategory> UpdateSubCategory([FromBody] SubCategory subCategory)
        {
            // Check if SubCategory exists
            if (string.IsNullOrEmpty(subCategory.Id))
                return null;
            
            // Update
            _context.Add(subCategory);
            await _context.SaveChangesAsync();

            return subCategory;
        }
        
        // DELETE -> /api/subcategories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(string id)
        {
            // Find sub-category in DB
            var subCategory = _context.SubCategories.FirstOrDefault(sc => sc.Id.Equals(id));
            
            // If sub-category does not exists
            if (subCategory == null)
                return NotFound();

            // Mark as deleted
            subCategory.Deleted = true;
            
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}