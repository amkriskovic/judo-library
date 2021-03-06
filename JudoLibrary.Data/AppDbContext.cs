﻿using JudoLibrary.Models;
using JudoLibrary.Models.Moderation;
using Microsoft.EntityFrameworkCore;

namespace JudoLibrary.Data
{
    public class AppDbContext : DbContext
    {
        // Passing options to DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tables
        public DbSet<Category> Categories { get; set; }
        public DbSet<TechniqueCategory> TechniqueCategories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<TechniqueSubCategory> TechniqueSubCategories { get; set; }
        public DbSet<TechniqueCounter> TechniqueCounters { get; set; }
        public DbSet<TechniqueSetupAttack> TechniqueSetupAttacks { get; set; }
        public DbSet<TechniqueFollowupAttack> TechniqueFollowupAttacks { get; set; }
        public DbSet<Technique> Techniques { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<SubmissionVote> SubmissionVotes { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<ModerationItem> ModerationItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }

        // Fluent API configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TechniqueCategory>()
                .HasKey(x => new {x.CategoryId, x.TechniqueId});
            
            modelBuilder.Entity<TechniqueSubCategory>()
                .HasKey(x => new {x.SubCategoryId, x.TechniqueId}); 
            
            // Creating composite key for TechniqueSetupAttack
            modelBuilder.Entity<TechniqueSetupAttack>()
                .HasKey(tsa => new {tsa.TechniqueId, tsa.SetUpAttackId});

            // Creating composite key for TechniqueFollowupAttack
            modelBuilder.Entity<TechniqueFollowupAttack>()
                .HasKey(tfa => new {tfa.TechniqueId, tfa.FollowUpAttackId});

            // Creating composite key for TechniqueCounter
            modelBuilder.Entity<TechniqueCounter>()
                .HasKey(tc => new {tc.TechniqueId, tc.CounterId});

            // TechniqueSetupAttack has One Technique with Many SetUpAttacks and has ForeignKey TechniqueId
            modelBuilder.Entity<TechniqueSetupAttack>()
                .HasOne(tsa => tsa.Technique)
                .WithMany(t => t.SetUpAttacks) // One Technique can have Many SetUpAttacks
                .HasForeignKey(tsa => tsa.TechniqueId);

            // TechniqueFollowupAttack has One Technique with Many FollowUpAttacks and has ForeignKey TechniqueId
            modelBuilder.Entity<TechniqueFollowupAttack>()
                .HasOne(tfa => tfa.Technique)
                .WithMany(t => t.FollowUpAttacks) // One Technique can have Many FollowUpAttacks
                .HasForeignKey(tsa => tsa.TechniqueId);

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