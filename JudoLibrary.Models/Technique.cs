using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    public class Technique : VersionedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<TechniqueCategory> TechniqueCategories { get; set; }
        
        public IList<TechniqueSubCategory> TechniqueSubCategories { get; set; } = new List<TechniqueSubCategory>(); 
        public IList<TechniqueSetupAttack> SetUpAttacks { get; set; } = new List<TechniqueSetupAttack>(); // One Technique can have Many SetUpAttacks
        public IList<TechniqueFollowupAttack> FollowUpAttacks { get; set; } = new List<TechniqueFollowupAttack>(); // One Technique can have Many FollowUpAttacks
        public IList<TechniqueCounter> Counters { get; set; } = new List<TechniqueCounter>(); // One Technique can have Many Counters 
    }
}