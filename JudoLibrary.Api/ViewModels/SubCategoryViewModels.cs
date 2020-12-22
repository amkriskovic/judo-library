using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Api.ViewModels
{
    public static class SubCategoryViewModels
    {
        public static readonly Func<SubCategory, object> CreateFlat = FlatProjection.Compile();
        public static Expression<Func<SubCategory, object>> FlatProjection =>
            subCategory => new
            {
                subCategory.Id,
                subCategory.Name,
                subCategory.Slug,
                subCategory.State,
                subCategory.Version,
            };
        
        public static readonly Func<SubCategory, object> Create = Projection.Compile();
        public static Expression<Func<SubCategory, object>> Projection =>
            subCategory => new
            {
                subCategory.Id,
                subCategory.Name,
                subCategory.Slug,
                subCategory.Description,
                subCategory.Version,
                subCategory.State,
                Updated = subCategory.Updated.ToLocalTime().ToString("HH:mm dd/MM/yyyy"),
                Techniques = subCategory.Techniques.AsQueryable()
                    .Where(x => x.Active)
                    .Select(x => x.TechniqueId)
                    .ToList()
            };
    }
}