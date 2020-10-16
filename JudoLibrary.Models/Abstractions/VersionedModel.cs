using System;

namespace JudoLibrary.Models.Abstractions
{
    // Abstract class that represents version of specific thing. It provides which version it is,
    // is it active or not, and time stamp for particular version
    public abstract class VersionedModel : TemporalModel
    {
        public int Version { get; set; } // Default = 0
        public bool Active { get; set; } // Default = false
        public DateTime TimeStamp { get; set; }
    }
}