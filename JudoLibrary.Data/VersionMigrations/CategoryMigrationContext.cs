using System.Linq;
using JudoLibrary.Models;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Data.VersionMigrations
{
    class CategoryMigrationContext : IEntityMigrationContext
    {
        private readonly AppDbContext _ctx;

        public CategoryMigrationContext(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public IQueryable<VersionedModel> GetSource()
        {
            return _ctx.Categories;
        }

        public void MigrateTechniqueRelationships(int current, int target)
        {
            if (current > 0)
            {
                var relationships = _ctx.TechniqueCategories
                    .Where(x => x.CategoryId == current)
                    .ToList();

                foreach (var relationship in relationships)
                {
                    relationship.Active = false;
                    _ctx.Add(new TechniqueCategory
                    {
                        CategoryId = target,
                        TechniqueId = relationship.TechniqueId,
                        Active = true,
                    });
                }
            }
        }
    }
}