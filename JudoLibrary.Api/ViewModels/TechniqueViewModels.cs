using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Api.ViewModels
{
    public static class TechniqueViewModels
    {
        // Default ViewModel
        // Expression holds information about function, returning a function that later can be traversed
        // EF will take Expression and translate it into Select statement
        public static Expression<Func<Technique, object>> Default =>
            // Accept TechniqueForm and return object, this function becomes object, we are mapping stuff to TechniqueForm(technique) object
            technique => new
            {
                technique.Id,
                technique.Name,
                technique.Description,
                technique.SubCategoryId, // Id
                Counters = technique.Counters.Select(tc => tc.CounterId), // Selecting Counters for TechniqueForm
                SetUpAttacks = technique.SetUpAttacks.Select(tsa => tsa.SetUpAttackId), // Selecting Counters for TechniqueForm
                FollowUpAttacks = technique.FollowUpAttacks.Select(tfa => tfa.FollowUpAttackId) // Selecting Counters for TechniqueForm
            };
    }
}