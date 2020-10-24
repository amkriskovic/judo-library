using System;
using System.Linq;
using JudoLibrary.Models;
using JudoLibrary.Models.Abstractions;
using JudoLibrary.Models.Moderation;

namespace JudoLibrary.Data
{
    public class VersionMigrationContext
    {
        private readonly AppDbContext _ctx;

        public VersionMigrationContext(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // Migrating based on moderationItem that's passed as arg
        public void Migrate(ModerationItem moderationItem)
        {
            // Getting the source based on moderationItem Type that's passed => we end up with IQue<VersionedModel> of type that's
            // passed in as argument -> moderationItem
            var source = GetSource(moderationItem.Type);

            // * Current and Target are not version, they are identifiers

            // Grab the current version from source => where VM Id is equal to moderationItem Current which represents Current
            // Version of item that we are moderating
            var current = source.FirstOrDefault(vm => vm.Id == moderationItem.Current);

            // Grab the next version (target) from source => where VM Id is equal to moderationItem Target
            // Current =>> Target
            var target = source.FirstOrDefault(vm => vm.Id == moderationItem.Target);

            // If target is null -> NotFound
            if (target == null) throw new InvalidOperationException("Target not found");

            // If current version is NOT null
            if (current != null)
            {
                // If the target version - current version is less than or equal to 0, we have a problem
                // For example there could be same multiple versions of Technique e.g. 2, and if we approve one of them
                // the rest should be invalid because 2 - 2 == 0 =>> problem/invalid
                if (target.Version - current.Version <= 0)
                {
                    throw new InvalidVersionException(
                        $"Current Version is {current.Version}, Target Version is {target.Version}, for {moderationItem.Type}");
                }

                // Deactivate current version => no longer active
                current.Active = false;

                // Grab all current moderation items => where MI is not Deleted,
                // and MI Type matches the moderationItem Type -> Thing that we are moderating (make sure that they are same types)
                // * and where MI Id is not equal to moderationItem Id, we dont bump the MI Id that we are currently using 
                var outdatedModerationItems = _ctx.ModerationItems
                    .Where(mi => !mi.Deleted && mi.Type == moderationItem.Type && mi.Id != moderationItem.Id)
                    .ToList();

                // Loop over outdatedModerationItems => outdated
                foreach (var outdatedModerationItem in outdatedModerationItems)
                {
                    // Get the target Id and assign it to outdatedModerationItem current so it points to new version => target
                    outdatedModerationItem.Current = target.Id;
                }
            }

            // Activate target => VersionModel
            target.Active = true;

            // Call MigrateTechniqueRelationships func, with provided original modItem: Current, Target and Type
            MigrateTechniqueRelationships(moderationItem.Current, moderationItem.Target, moderationItem.Type);
        }

        // Getting Source VersionedModel based on type provided which evaluates to some DbSet<Entity>
        private IQueryable<VersionedModel> GetSource(string type)
        {
            // If the target type is equal to Technique
            // Return DbSet<Techniques>
            if (type == ModerationTypes.Technique)
            {
                // Returns DbSet<Techniques>
                return _ctx.Techniques;
            }

            // If it's not supported type
            throw new ArgumentException(nameof(type));
        }

        private void MigrateTechniqueRelationships(int current, int target, string type)
        {
            // If the target type is equal to Technique
            if (type == ModerationTypes.Technique)
            {
                // If the current version is greater than 0, perform "deactivation" for particular Technique type (context)
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
            else
            {
                // If it's not supported type
                throw new ArgumentException(nameof(type));
            }
        }

        public class InvalidVersionException : Exception
        {
            public InvalidVersionException(string message) : base(message)
            {
            }
        }
    }
}