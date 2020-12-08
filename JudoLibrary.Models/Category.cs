using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    public class Category : VersionedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<TechniqueCategory> Techniques { get; set; } = new List<TechniqueCategory>();
    }
}