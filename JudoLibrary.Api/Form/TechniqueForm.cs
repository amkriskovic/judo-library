using System.Collections.Generic;
using JudoLibrary.Models;

namespace JudoLibrary.Api.Form
{
    public class TechniqueForm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } // One TechniqueForm can have One Category
        public string SubCategory { get; set; } // One TechniqueForm can have One SubCategory
        public IEnumerable<string> SetUpAttacks { get; set; }
        public IEnumerable<string> FollowUpAttacks { get; set; }
        public IEnumerable<string> Counters { get; set; }
    }
}