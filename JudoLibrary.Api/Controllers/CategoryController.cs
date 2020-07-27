using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET -> /api/categories
        [HttpGet]
        public IEnumerable<Category> GetAllCategories() => _context.Categories.ToList();
        
        // GET -> /api/categories/{id}c
        [HttpGet("{id}")]
        public Category GetCategory(string id) => _context.Categories
            .FirstOrDefault(c => c.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        
        // GET -> /api/categories/{id}/{subcategories}
        // Get all subcategories for particular category
        [HttpGet("{id}/subcategories")]
        public IEnumerable<SubCategory> GetAllSubCategoriesForCategory(string id) =>
            _context.SubCategories
                .Where(sc => sc.CategoryId.Equals(id))
                .ToList();
        
        // POST -> /api/categories
        // Create category, sending json from the body of the request
        [HttpPost]
        public async Task<Category> CreateCategory([FromBody] Category category)
        {
            // Create Category -> Id, replacing white spaces with dashes, to lower ==> slug
            category.Id = category.Name.Replace(" ", "-").ToLowerInvariant();
            
            // Add category to DB
            _context.Add(category);
            
            // Saves changes async so it doesn't wait for actual saving time to DB, await prevents blocking UI
            await _context.SaveChangesAsync();

            return category;
        }

        // PUT -> /api/categories
        [HttpPut]
        public async Task<Category> UpdateCategory([FromBody] Category category)
        {
            // Check if category exists
            if (string.IsNullOrEmpty(category.Id))
                return null;

            // Update
            _context.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }
        
        // DELETE -> /api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            // Grab category by Id
            var category = _context.Categories.FirstOrDefault(c => c.Id.Equals(id));
            
            if (category == null)
                return NotFound();

            // Mark field Deleted as true
            category.Deleted = true;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}