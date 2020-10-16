namespace JudoLibrary.Models.Abstractions
{
    // Abstract class intended for slug functionality
    public abstract class SlugModel : VersionedModel
    {
        public string Slug { get; set; }
    }
}