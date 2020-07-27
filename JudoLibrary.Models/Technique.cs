using System.Collections.Generic;

namespace JudoLibrary.Models
{
    public class Technique : BaseModel<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubCategoryId { get; set; } // One Technique can have One SubCategory

        public IList<TechniqueSetupAttack> SetUpAttacks { get; set; } // One Technique can have Many SetUpAttacks
        public IList<TechniqueFollowupAttack> FollowUpAttacks { get; set; } // One Technique can have Many FollowUpAttacks

        public IList<TechniqueCounter> Counters { get; set; } // One Technique can have Many Counters 
    }
}