using System;
using System.Linq;
using JudoLibrary.Models;
using JudoLibrary.Models.Moderation;

namespace JudoLibrary.Data.VersionMigrations
{
    public class VersionMigrationContext
    {
        private readonly AppDbContext _ctx;

        public VersionMigrationContext(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        private ModerationItem ModerationItem { get; set; }
        private IEntityMigrationContext EntityMigrationContext { get; set; }

        public VersionMigrationContext Setup(ModerationItem moderationItem)
        {
            ModerationItem = moderationItem ?? throw new ArgumentException(nameof(moderationItem));
            EntityMigrationContext = moderationItem.Type switch
            {
                ModerationTypes.Technique => new TechniqueMigrationContext(_ctx),
                ModerationTypes.Category => new CategoryMigrationContext(_ctx),
                ModerationTypes.SubCategory => new SubCategoryMigrationContext(_ctx),
                _ => throw new ArgumentException(nameof(moderationItem.Type))

            };
            return this;
        }

        // Migrating based on moderationItem that's passed as arg
        public void Migrate()
        {
            if (ModerationItem == null) throw new NullReferenceException(nameof(ModerationItem));
            if (EntityMigrationContext == null) throw new NullReferenceException(nameof(EntityMigrationContext));
            
            // Getting the source based on moderationItem Type that's passed => we end up with IQue<VersionedModel> of type that's
            // passed in as argument -> moderationItem
            var source = EntityMigrationContext.GetSource();

            // Grab the current version from source => where VM Id is equal to moderationItem Current which represents Current
            // Version of item that we are moderating
            var current = source.FirstOrDefault(vm => vm.Id == ModerationItem.Current);

            // Grab the next version (target) from source => where VM Id is equal to moderationItem Target
            // Current =>> Target
            var target = source.FirstOrDefault(vm => vm.Id == ModerationItem.Target);

            // If target is null -> NotFound
            if (target == null) throw new InvalidOperationException("Target not found");

            // If current version is NOT null
            if (current != null)
            {
                
                var newVersion = !current.Slug.Equals(target.Slug, StringComparison.InvariantCultureIgnoreCase);
                var outdatedVersion = target.Version - current.Version <= 0;
                if (outdatedVersion && !newVersion)
                {
                    throw new InvalidVersionException(
                        $"Current Version is {current.Version}, Target Version is {target.Version}, for {ModerationItem.Type}");
                }

                // Deactivate current version => no longer active
                current.State = VersionState.Outdated;

                // Grab all current moderation items => where MI is not Deleted,
                // and MI Type matches the moderationItem Type -> Thing that we are moderating (make sure that they are same types)
                // * and where MI Id is not equal to moderationItem Id, we dont bump the MI Id that we are currently using 
                var outdatedModerationItems = _ctx.ModerationItems
                    .Where(mi => !mi.Deleted && mi.Type == ModerationItem.Type && mi.Id != ModerationItem.Id)
                    .ToList();

                // Loop over outdatedModerationItems => outdated
                foreach (var outdatedModerationItem in outdatedModerationItems)
                {
                    // Get the target Id and assign it to outdatedModerationItem current so it points to new version => target
                    outdatedModerationItem.Current = target.Id;
                }
            }

            // Activate target => VersionModel
            target.State = VersionState.Live;

            // Call MigrateTechniqueRelationships func, with provided original modItem: Current, Target
            EntityMigrationContext.MigrateTechniqueRelationships(ModerationItem.Current, ModerationItem.Target);
        }

        public class InvalidVersionException : Exception
        {
            public InvalidVersionException(string message) : base(message)
            {
            }
        }
    }
}