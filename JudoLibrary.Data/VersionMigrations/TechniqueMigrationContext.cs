using System.Linq;
using JudoLibrary.Models.Abstractions;
using Microsoft.EntityFrameworkCore;

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
                var currentTechnique = _ctx.Techniques
                    .Include(x => x.TechniqueCategories)
                    .Include(x => x.TechniqueSubCategories)
                    .Include(x => x.SetUpAttacks)
                    .Include(x => x.FollowUpAttacks)
                    .Include(x => x.Counters)
                    .FirstOrDefault(x => x.Id == current);

                foreach (var category in currentTechnique.TechniqueCategories)
                    category.Active = false;
                foreach (var subCategory in currentTechnique.TechniqueSubCategories)
                    subCategory.Active = false;
                foreach (var setupAttack in currentTechnique.SetUpAttacks)
                    setupAttack.Active = false;
                foreach (var followupAttack in currentTechnique.FollowUpAttacks)
                    followupAttack.Active = false;
                foreach (var counter in currentTechnique.Counters)
                    counter.Active = false;
                    
                // _ctx.TechniqueSetupAttacks
                //     .Where(tsa => tsa.SetUpAttackId == current || tsa.TechniqueId == current)
                //     .ToList()
                //     .ForEach(tsa => tsa.Active = false);
                //     
                // _ctx.TechniqueFollowupAttacks
                //     .Where(tfa => tfa.FollowUpAttackId == current || tfa.TechniqueId == current)
                //     .ToList()
                //     .ForEach(tfa => tfa.Active = false);
                //     
                // _ctx.TechniqueCounters
                //     .Where(tc => tc.CounterId == current || tc.TechniqueId == current)
                //     .ToList()
                //     .ForEach(tc => tc.Active = false);
            }
                
            // _ctx.TechniqueSetupAttacks
            //     .Where(tsa => tsa.SetUpAttackId == target || tsa.TechniqueId == target)
            //     .ToList()
            //     .ForEach(tsa => tsa.Active = true);
            //     
            // _ctx.TechniqueFollowupAttacks
            //     .Where(tfa => tfa.FollowUpAttackId == target || tfa.TechniqueId == target)
            //     .ToList()
            //     .ForEach(tfa => tfa.Active = true);
            //     
            // _ctx.TechniqueCounters
            //     .Where(tc => tc.CounterId == target || tc.TechniqueId == target)
            //     .ToList()
            //     .ForEach(tc => tc.Active = true);
            
            var targetTechnique = _ctx.Techniques
                .Include(x => x.TechniqueCategories)
                .Include(x => x.TechniqueSubCategories)
                .Include(x => x.SetUpAttacks)
                .Include(x => x.FollowUpAttacks)
                .Include(x => x.Counters)
                .FirstOrDefault(x => x.Id == target);

            foreach (var category in targetTechnique.TechniqueCategories)
                category.Active = true;
            foreach (var subCategory in targetTechnique.TechniqueSubCategories)
                subCategory.Active = true;
            foreach (var setupAttack in targetTechnique.SetUpAttacks)
                setupAttack.Active = true;
            foreach (var followupAttack in targetTechnique.FollowUpAttacks)
                followupAttack.Active = true;
            foreach (var counter in targetTechnique.Counters)
                counter.Active = true;
        }
    }
}