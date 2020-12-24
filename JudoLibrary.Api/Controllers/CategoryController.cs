using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/categories")]
    public class CategoryController : ApiController
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET -> /api/categories
        [HttpGet]
        public IEnumerable<object> GetAllCategories() => _context.Categories
            .Where(x => !x.Deleted && x.State == VersionState.Live)
            .Select(CategoryViewModels.Projection)
            .ToList();

        // GET -> /api/categories/{value}
        [HttpGet("{value}")]
        public object GetCategory(string value) =>
            _context.Categories
                .WhereIdOrSlug(value)
                .Select(CategoryViewModels.Projection)
                .FirstOrDefault();

        // GET -> /api/categories/{slug}/history
        [HttpGet("{slug}/history")]
        public IEnumerable<object> GetCategoryHistory(string slug)
        {
            return _context.Categories
                .Where(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && x.State != VersionState.Staged)
                .Select(CategoryViewModels.Projection)
                .ToList();
        }


        // GET -> /api/categories/{id}/subcategories
        // Get all subcategories for particular category | Passing category Id as param
        [HttpGet("{id}/subcategories")]
        public IEnumerable<SubCategory> GetAllSubCategoriesForCategory(string id) =>
            _context.SubCategories
                .Where(sc => sc.CategoryId.Equals(id))
                .ToList();

        // GET -> /api/categories/{id}/techniques
        // Get all techniques for particular category | Passing category Id as param
        // [HttpGet("{id}/techniques")]
        // public IEnumerable<Technique> GetAllTechniquesForCategory(int id) =>
        //     _context.Techniques
        //         .Where(t => t.Category.Equals(id))
        //         .ToList();

        // POST -> /api/categories
        // Created category, sending json from the body of the request
        [HttpPost]
        [Authorize(JudoLibraryConstants.Policies.Mod)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryForm form)
        {
            var category = new Category
            {
                Slug = form.Name.Replace(" ", "-").ToLowerInvariant(),
                Name = form.Name,
                Description = form.Description,
                Version = 1,
                UserId = UserId
            };

            // Add category to DB
            _context.Add(category);

            // Saves changes async so it doesn't wait for actual saving time to DB, await prevents blocking UI
            await _context.SaveChangesAsync();

            _context.ModerationItems.Add(new ModerationItem
            {
                Target = category.Id,
                UserId = UserId,
                Type = ModerationTypes.Category,
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT -> /api/categories
        [HttpPut]
        [Authorize(JudoLibraryConstants.Policies.Mod)]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryForm form)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == form.Id);

            if (category == null)
            {
                return NoContent();
            }

            var newCategory = new Category
            {
                Slug = category.Slug,
                Name = category.Name,
                Description = form.Description,
                Version = category.Version + 1,
                UserId = UserId,
            };

            // Add category to DB
            _context.Add(newCategory);

            // Saves changes async so it doesn't wait for actual saving time to DB, await prevents blocking UI
            await _context.SaveChangesAsync();

            _context.ModerationItems.Add(new ModerationItem
            {
                Current = category.Id,
                Target = newCategory.Id,
                UserId = UserId,
                Type = ModerationTypes.Category,
            });

            await _context.SaveChangesAsync();

            return Ok();
        }
        
        // DELETE -> /api/categories
        [HttpDelete("{id}")]
        [Authorize(JudoLibraryConstants.Policies.Mod)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!_context.Categories.Any(x => x.Id == id))
            {
                return NoContent();
            }

            _context.ModerationItems.Add(new ModerationItem
            {
                Current = id,
                UserId = UserId,
                Type = ModerationTypes.Category,
            });

            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpPut("staged")]
        [Authorize]
        public async Task<IActionResult> UpdateStaged([FromBody] UpdateCategoryForm form)
        {
            var category = _context.Categories
                .FirstOrDefault(x => x.Id == form.Id);

            if (category == null) return NoContent();
            if (category.UserId != UserId) return BadRequest("Can't edit this category.");

            category.Description = form.Description;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}