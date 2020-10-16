using System.Linq;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Data
{
    public static class QueryExtensions
    {
        // This IQueryable is working for VersionedModel class =>> where, returns maximum version which is Int
        // Specifying offset which is used to bump up the version
        public static int LatestVersion<TSource>(this IQueryable<TSource> source, int offset = 0)
            where TSource : VersionedModel
        {
            return source.Max(x => x.Version) + offset;
        }
    }
}