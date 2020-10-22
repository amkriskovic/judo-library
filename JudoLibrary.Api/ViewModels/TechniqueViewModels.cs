using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Api.ViewModels
{
    public static class TechniqueViewModels
    {
        // Assign to create delegate, projection expression that we compile, so we can use it in controller
        public static readonly Func<Technique, object> Create = Projection.Compile();

        // Projection ViewModel
        // Expression holds information about function, returning a function that later can be traversed
        // EF will take Expression and translate it into Select statement
        public static Expression<Func<Technique, object>> Projection =>
            // Accept TechniqueForm and return object, this function becomes object, we are mapping stuff to TechniqueForm(technique) object
            technique => new
            {
                technique.Id,
                technique.Slug,
                technique.Name,
                technique.Description,
                technique.Category, // Id
                technique.SubCategory, // Id
                technique.Version,
                technique.Active,
                SetUpAttacks = technique.SetUpAttacks
                    .AsQueryable()
                    .Where(x => x.Active)
                    .Select(tsa => tsa.SetUpAttackId) // Selecting Counters for TechniqueForm
                    .ToList(),
                FollowUpAttacks = technique.FollowUpAttacks
                    .AsQueryable()
                    .Where(x => x.Active)
                    .Select(tfa => tfa.FollowUpAttackId) // Selecting Counters for TechniqueForm
                    .ToList(),
                Counters = technique.Counters
                    .AsQueryable()
                    .Where(x => x.Active)
                    .Select(tc => tc.CounterId) // Selecting Counters for TechniqueForm
                    .ToList()
            };
    }
}