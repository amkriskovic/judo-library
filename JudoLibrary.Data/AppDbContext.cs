using JudoLibrary.Models;
using JudoLibrary.Models.Moderation;
using Microsoft.EntityFrameworkCore;

namespace JudoLibrary.Data
{
    public class AppDbContext : DbContext
    {
        // Passing options to DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        // Tables
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<TechniqueRelationships> TechniqueRelationships { get; set; }
        public DbSet<TechniqueCounter> TechniqueCounters { get; set; }
        public DbSet<Technique> Techniques { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<ModerationItem> ModerationItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        
        // Fluent API configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Creating composite key for TechniqueCounter
            modelBuilder.Entity<TechniqueCounter>()
                .HasKey(tc => new {tc.TechniqueId, tc.CounterId});
            
            // Creating composite key for TechniqueSetupAttack
            modelBuilder.Entity<TechniqueRelationships>()
                .HasKey(x => new {x.SetUpAttackId, x.FollowUpAttackId});

            // TechniqueSetupAttack has One Technique with Many SetUpAttacks and has ForeignKey TechniqueId
            modelBuilder.Entity<TechniqueRelationships>()
                .HasOne(x => x.SetUpAttack)
                .WithMany(t => t.FollowUpAttacks) // One Technique can have Many SetUpAttacks
                .HasForeignKey(x => x.SetUpAttackId);
            
            // TechniqueFollowupAttack has One Technique with Many FollowUpAttacks and has ForeignKey TechniqueId
            modelBuilder.Entity<TechniqueRelationships>()
                .HasOne(x => x.FollowUpAttack)
                .WithMany(t => t.SetUpAttacks) // One Technique can have Many FollowUpAttacks
                .HasForeignKey(x => x.FollowUpAttackId);
            
            // TechniqueCounter has One Technique with Many TechniqueCounters and ForeignKey TechniqueId
            modelBuilder.Entity<TechniqueCounter>()
                .HasOne(tc => tc.Technique)
                .WithMany(t => t.Counters) // One Technique can have Many TechniqueCounters
                .HasForeignKey(tc => tc.TechniqueId);

            // // Defining TechniqueId in Submission table as ForeignKey
            // modelBuilder.Entity<Submission>()
            //     .HasOne<Technique>()
            //     .WithMany()
            //     .HasForeignKey(s => s.TechniqueId);
            
            // // Defining CategoryId in SubCategory table as ForeignKey
            // modelBuilder.Entity<SubCategory>()
            //     .HasOne<Category>()
            //     .WithMany()
            //     .HasForeignKey(sc => sc.CategoryId);
        }
    }
}