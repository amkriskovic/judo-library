using System.Linq;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Data.VersionMigrations
{
    public interface IEntityMigrationContext
    {
        IQueryable<VersionedModel> GetSource();
        void MigrateTechniqueRelationships(int current, int target);
    }
}