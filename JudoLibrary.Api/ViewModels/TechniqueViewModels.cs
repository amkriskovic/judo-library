using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Api.ViewModels
{
    public static class TechniqueViewModels
    {
        public static readonly Func<Technique, object> Create = Projection.Compile();
        public static Expression<Func<Technique, object>> Projection =>
            // Accept TechniqueForm and return object, this function becomes object, we are mapping stuff to TechniqueForm(technique) object
            technique => new
            {
                technique.Id,
                technique.Slug,
                technique.Name,
                technique.Description,
                Category = technique.TechniqueCategories.AsQueryable()
                    .Where(x => x.Active)
                    .Select(x => x.CategoryId)
                    .FirstOrDefault(),
                SubCategory = technique.TechniqueSubCategories.AsQueryable()
                    .Where(x => x.Active)
                    .Select(x => x.SubCategoryId)
                    .FirstOrDefault(),
                technique.Version,
                SetUpAttacks = technique.SetUpAttacks
                    .AsQueryable()
                    .Where(x => x.Active)
                    .Select(x => x.SetUpAttackId) 
                    .ToList(),
                FollowUpAttacks = technique.FollowUpAttacks
                    .AsQueryable()
                    .Where(x => x.Active)
                    .Select(x => x.FollowUpAttackId) 
                    .ToList(),
                Counters = technique.Counters
                    .AsQueryable()
                    .Where(x => x.Active)
                    .Select(tc => tc.CounterId) 
                    .ToList(),
                User = UserViewModel.CreateFlat(technique.User)
            };
        
        // Projection ViewModel
        // Expression holds information about function, returning a function that later can be traversed
        // EF will take Expression and translate it into Select statement
        public static readonly Func<Technique, object> CreateFull = FullProjection.Compile();
        public static Expression<Func<Technique, object>> FullProjection =>
            // Accept TechniqueForm and return object, this function becomes object, we are mapping stuff to TechniqueForm(technique) object
            technique => new
            {
                technique.Id,
                technique.Slug,
                technique.Name,
                technique.Description,
                technique.State,
                Category = technique.TechniqueCategories.AsQueryable()
                    .OrderByDescending(x => x.Active)
                    .Select(x => x.CategoryId)
                    .FirstOrDefault(),
                SubCategory = technique.TechniqueSubCategories.AsQueryable()
                    .Select(x => x.SubCategoryId)
                    .FirstOrDefault(),
                technique.Version,
                SetUpAttacks = technique.SetUpAttacks
                    .AsQueryable()
                    .Select(x => x.SetUpAttackId) 
                    .ToList(),
                FollowUpAttacks = technique.FollowUpAttacks
                    .AsQueryable()
                    .Select(x => x.FollowUpAttackId)
                    .ToList(),
                Counters = technique.Counters
                    .AsQueryable()
                    .Select(tc => tc.CounterId) // Selecting Counters for TechniqueForm
                    .ToList(),
                User = UserViewModel.CreateFlat(technique.User)
            };
        
        public static readonly Func<Technique, object> CreateFlat = FlatProjection.Compile();
        public static Expression<Func<Technique, object>> FlatProjection =>
            technique => new
            {
                technique.Id,
                technique.Slug,
                technique.Name,
                technique.Description,
                technique.State,
                technique.Version,
            };
    }
}