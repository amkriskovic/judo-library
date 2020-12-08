﻿using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Api.ViewModels
{
    public static class CategoryViewModels
    {
        public static readonly Func<Category, object> CreateFlat = FlatProjection.Compile();
        public static Expression<Func<Category, object>> FlatProjection =>
            category => new
            {
                category.Id,
                category.Name,
                category.Slug,
                category.Description,
                category.Version,
            };
        
        
        public static readonly Func<Category, object> Create = Projection.Compile();
        public static Expression<Func<Category, object>> Projection =>
            category => new
            {
                category.Id,
                category.Name,
                category.Slug,
                category.Description,
                category.Version,
                Updated = category.Updated.ToLocalTime().ToString("HH:mm dd/MM/yyyy"),
                Techniques = category.Techniques.AsQueryable().Select(x => x.TechniqueId).ToList()
            };
    }
}