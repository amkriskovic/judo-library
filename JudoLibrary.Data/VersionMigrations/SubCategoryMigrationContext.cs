using System;
using System.Linq;
using JudoLibrary.Models;
using JudoLibrary.Models.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace JudoLibrary.Data.VersionMigrations
{
    public class SubCategoryMigrationContext : IEntityMigrationContext
    {
        private readonly AppDbContext _ctx;

        public SubCategoryMigrationContext(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<VersionedModel> GetSource()
        {
            return _ctx.SubCategories;
        }

        public void MigrateTechniqueRelationships(int current, int target)
        {
            if (current > 0)
            {
                var relationships = _ctx.TechniqueSubCategories
                    .Include(x => x.Technique)
                    .Where(x => x.SubCategoryId == current)
                    .ToList();

                foreach (var relationship in relationships)
                {
                    relationship.Active = false;
                    _ctx.Add(new TechniqueSubCategory
                    {
                        SubCategoryId = target,
                        TechniqueId = relationship.TechniqueId,
                        Active = relationship.Active,
                    });
                    if (relationship.Technique.State == VersionState.Staged)
                        _ctx.Remove(relationship);
                    else
                        relationship.Active = false;
                }
            }
        }

        public void VoidRelationships(int id)
        {
            throw new NotImplementedException();
        }
    }
}