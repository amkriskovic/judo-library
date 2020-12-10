using System.Collections.Generic;
using JudoLibrary.Models;

namespace JudoLibrary.Api.Form
{
    public class CreateTechniqueForm
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Category { get; set; } // One TechniqueForm can have One Category
        public int SubCategory { get; set; } // One TechniqueForm can have One SubCategory
        public IEnumerable<int> SetUpAttacks { get; set; }
        public IEnumerable<int> FollowUpAttacks { get; set; }
        public IEnumerable<int> Counters { get; set; }
    }
}