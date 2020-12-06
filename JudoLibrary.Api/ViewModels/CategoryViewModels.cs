﻿using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Api.ViewModels
{
    public static class CategoryViewModels
    {
        public static readonly Func<Category, object> Create = Projection.Compile();
        public static Expression<Func<Category, object>> Projection =>
            category => new
            {
                category.Id,
                category.Name,
                category.Description,
                Updated = category.Updated.ToLocalTime().ToString("HH:mm dd/MM/yyyy"),
                Techniques = category.Techniques.AsQueryable().Select(x => x.Slug).ToList()
            };
    }
}