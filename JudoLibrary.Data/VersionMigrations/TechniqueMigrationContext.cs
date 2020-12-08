using System.Linq;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Data.VersionMigrations
{
    class TechniqueMigrationContext : IEntityMigrationContext
    {
        private readonly AppDbContext _ctx;

        public TechniqueMigrationContext(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public IQueryable<VersionedModel> GetSource()
        {
            return _ctx.Techniques;
        }

        public void MigrateTechniqueRelationships(int current, int target)
        {
            if (current > 0)
            {
                _ctx.TechniqueSetupAttacks
                    .Where(tsa => tsa.SetUpAttackId == current || tsa.TechniqueId == current)
                    .ToList()
                    .ForEach(tsa => tsa.Active = false);
                    
                _ctx.TechniqueFollowupAttacks
                    .Where(tfa => tfa.FollowUpAttackId == current || tfa.TechniqueId == current)
                    .ToList()
                    .ForEach(tfa => tfa.Active = false);
                    
                _ctx.TechniqueCounters
                    .Where(tc => tc.CounterId == current || tc.TechniqueId == current)
                    .ToList()
                    .ForEach(tc => tc.Active = false);
            }
                
            _ctx.TechniqueSetupAttacks
                .Where(tsa => tsa.SetUpAttackId == target || tsa.TechniqueId == target)
                .ToList()
                .ForEach(tsa => tsa.Active = true);
                
            _ctx.TechniqueFollowupAttacks
                .Where(tfa => tfa.FollowUpAttackId == target || tfa.TechniqueId == target)
                .ToList()
                .ForEach(tfa => tfa.Active = true);
                
            _ctx.TechniqueCounters
                .Where(tc => tc.CounterId == target || tc.TechniqueId == target)
                .ToList()
                .ForEach(tc => tc.Active = true);
        }
    }
}