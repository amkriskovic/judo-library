using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    public class Video : TemporalModel
    {
        public string ThumbLink { get; set; }
        public string VideoLink { get; set; }
    }
}