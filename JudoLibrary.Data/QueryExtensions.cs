using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Data
{
    public static class QueryExtensions
    {
        public static IQueryable<Submission> PickSubmissions(this IQueryable<Submission> source, string order, int cursor)
        {
            // Exp, takes Submission return object, switching based on passed order string
            Expression<Func<Submission, object>> orderBySelector = order switch
            {
                "latest" => submission => submission.Created,
                "top" => submission => submission.UpVotes.Count,
                _ => _ => 1,
            };

            return source
                .OrderByDescending(orderBySelector)
                .Skip(cursor) // Skip (0)
                .Take(10); // Take 10 of the items per page
        }
    }
}