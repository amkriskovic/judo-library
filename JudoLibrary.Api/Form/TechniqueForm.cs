using System.Collections.Generic;
using JudoLibrary.Models;

namespace JudoLibrary.Api.Form
{
    public class TechniqueForm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } // One TechniqueForm can have One Category
        public string SubCategory { get; set; } // One TechniqueForm can have One SubCategory
        public IEnumerable<int> SetUpAttacks { get; set; }
        public IEnumerable<int> FollowUpAttacks { get; set; }
        public IEnumerable<int> Counters { get; set; }
    }
}