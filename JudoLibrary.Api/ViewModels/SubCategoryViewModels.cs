using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Api.ViewModels
{
    public static class SubCategoryViewModels
    {
        public static readonly Func<SubCategory, object> Create = Projection.Compile();
        public static Expression<Func<SubCategory, object>> Projection =>
            subCategory => new
            {
                subCategory.Id,
                subCategory.Name,
                subCategory.Description,
                Updated = subCategory.Updated.ToLocalTime().ToString("HH:mm dd/MM/yyyy"),
                Techniques = subCategory.Techniques.AsQueryable().Select(x => x.Slug).ToList()
            };
    }
}