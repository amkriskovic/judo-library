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
            .Where(x => !x.Deleted && x.State == VersionState.Live)
            .Select(SubCategoryViewModels.Projection)
            .ToList();
        
        // GET -> /api/subcategories/{value}
        [HttpGet("{value}")]
        public object GetSubCategory(string value) => _context.SubCategories
            .WhereIdOrSlug(value)
            .Select(SubCategoryViewModels.Projection)
            .FirstOrDefault();
        
        // GET -> /api/subcategories/{slug}/history
        [HttpGet("{slug}/history")]
        public IEnumerable<object> GetSubCategoryHistory(string slug)
        {
            return _context.SubCategories
                .Where(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && x.State != VersionState.Staged)
                .Select(SubCategoryViewModels.Projection)
                .ToList();
        }
        
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
        [Authorize(JudoLibraryConstants.Policies.Mod)]
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
        [Authorize(JudoLibraryConstants.Policies.Mod)]
        public async Task<IActionResult> UpdateSubCategory([FromBody] UpdateSubCategoryForm form)
        {
            var subcategory = _context.SubCategories.FirstOrDefault(x => x.Id == form.Id);

            if (subcategory == null)
            {
                return NoContent();
            }
            
            var newSubcategory = new SubCategory
            {
                Slug = subcategory.Slug,
                Name = subcategory.Name,
                Description = form.Description,
                CategoryId = form.CategoryId,
                UserId = UserId,
                Version = subcategory.Version + 1
            };
            
            newSubcategory.Version = subcategory.Slug.Equals(newSubcategory.Slug, StringComparison.InvariantCultureIgnoreCase)
                ? subcategory.Version + 1
                : 1;
            
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
        
        [HttpPut("{current}/{target}")]
        [Authorize(JudoLibraryConstants.Policies.Mod)]
        public async Task<IActionResult> Migrate(int current, int target)
        {
            var subCategoryCount = _context.SubCategories.Count(x => !x.Deleted
                                                               && x.State == VersionState.Live
                                                               && (x.Id == current || x.Id == target));
            if (subCategoryCount != 2)
            {
                return NoContent();
            }

            _context.ModerationItems.Add(new ModerationItem
            {
                Current = current,
                Target = target,
                UserId = UserId,
                Type = ModerationTypes.SubCategory,
            });
            
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpPut("staged")]
        [Authorize]
        public async Task<IActionResult> UpdateStaged([FromBody] UpdateSubCategoryForm form)
        {
            var subCategory = _context.SubCategories
                .FirstOrDefault(x => x.Id == form.Id);

            if (subCategory == null) return NoContent();
            if (subCategory.UserId != UserId) return BadRequest("Can't edit this SubCategory.");

            subCategory.Description = form.Description;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}