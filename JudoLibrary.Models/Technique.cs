using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    public class Technique : VersionedModel
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } // One Technique can have One Category
        public string SubCategory { get; set; } // One Technique can have One SubCategory

        public IList<TechniqueRelationships> SetUpAttacks { get; set; } = new List<TechniqueRelationships>(); // One Technique can have Many SetUpAttacks
        public IList<TechniqueRelationships> FollowUpAttacks { get; set; } = new List<TechniqueRelationships>(); // One Technique can have Many FollowUpAttacks
        public IList<TechniqueCounter> Counters { get; set; } = new List<TechniqueCounter>(); // One Technique can have Many Counters 
    }
}