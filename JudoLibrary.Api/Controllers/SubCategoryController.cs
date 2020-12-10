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

namespace JudoLibrary.Api.Controllers
{
    [Route("/api/subcategories")]
    public class SubCategoryController : ApiController
    {
        private readonly AppDbContext _context;

        public SubCategoryController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET -> /api/subcategories
        [HttpGet]
        public IEnumerable<object> GetAllSubCategories() => _context.SubCategories
            .Where(x => !x.Deleted && x.Active)
            .Select(SubCategoryViewModels.Projection)
            .ToList();
        
        // GET -> /api/subcategories/{value}
        [HttpGet("{value}")]
        public object GetSubCategory(string value) => _context.SubCategories
            .WhereIdOrSlug(value)
            .Select(SubCategoryViewModels.Projection)
            .FirstOrDefault();
        
        // GET -> /api/subcategories/{value}/techniques
        // Get all techniques for particular sub-category | Passing sub-category Id as param
        [HttpGet("{value}/techniques")]
        public IEnumerable<object> GetAllTechniquesForSubCategory(string value) => 
            _context.Techniques
            .WhereIdOrSlug(value)
            .Select(TechniqueViewModels.Projection)
            .ToList();
        
        // POST -> /api/subcategories
        // Created category, sending json from the body of the request
        [HttpPost]
        public async Task<IActionResult> CreateSubCategory([FromBody] CreateSubCategoryForm form)
        {
            var subcategory = new SubCategory
            {
                Slug = form.Name.Replace(" ", "-").ToLowerInvariant(),
                Name = form.Name,
                Description = form.Description,
                CategoryId = form.CategoryId,
                UserId = UserId,
            };
            
            _context.Add(subcategory);
            
            await _context.SaveChangesAsync();

            _context.Add(new ModerationItem
            {
                Target = subcategory.Id,
                UserId = UserId,
                Type = ModerationTypes.SubCategory
            });

            await _context.SaveChangesAsync();

            return Ok();
        }
        
        // PUT -> /api/subcategories
        // Created category, sending json from the body of the request
        [HttpPut]
        public async Task<IActionResult> UpdateSubCategory([FromBody] UpdateSubCategoryForm form)
        {
            var subcategory = _context.SubCategories.FirstOrDefault(x => x.Id == form.Id);

            if (subcategory == null)
            {
                return NoContent();
            }
            
            var newSubcategory = new SubCategory
            {
                Slug = form.Name.Replace(" ", "-").ToLowerInvariant(),
                Name = form.Name,
                Description = form.Description,
                CategoryId = form.CategoryId,
                UserId = UserId,
                Version = subcategory.Version + 1
            };
            
            _context.Add(newSubcategory);
            
            await _context.SaveChangesAsync();

            _context.Add(new ModerationItem
            {
                Current = subcategory.Id,
                Target = newSubcategory.Id,
                UserId = UserId,
                Type = ModerationTypes.SubCategory
            });

            await _context.SaveChangesAsync();

            return Ok();
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