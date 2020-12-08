using System;

namespace JudoLibrary.Models.Abstractions
{
    // Abstract class that represents version of specific thing. It provides which version it is,
    // is it active or not, and time stamp for particular version -> Used in version migration context
    public abstract class VersionedModel : Mutable<int>
    {
        public string Slug { get; set; }
        public int Version { get; set; } = 1; // Default = 0
        public bool Active { get; set; } // Default = false
    }
}