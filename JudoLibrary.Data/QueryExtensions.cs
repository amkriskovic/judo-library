using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Data
{
    public static class QueryExtensions
    {
        public static IQueryable<Submission> OrderSubmissions(
            this IQueryable<Submission> source, string order)
        {
            // Exp, takes Submission return object, switching based on passed order string
            Expression<Func<Submission, object>> orderBySelector = order switch
            {
                "latest" => submission => submission.Created,
                "top" => submission => submission.UpVotes.Count,
                _ => _ => 1,
            };

            return source.OrderByDescending(orderBySelector);
        }
        
        public static IQueryable<Comment> OrderComments(
            this IQueryable<Comment> source, string order)
        {
            if (order == "latest")
            {
                source = source.OrderByDescending(c => c.Created);
            } else if (order == "first")
            {
                source = source.OrderBy(c => c.Created);
            }

            return source;
        }
        
        public static IQueryable<T> OrderFeed<T>(this IQueryable<T> source, FeedQuery feedQuery)
        {
            if (source is IQueryable<Submission> submissionSource)
            {
                source = (IQueryable<T>) submissionSource.OrderSubmissions(feedQuery.Order);
            }
            else if(source is IQueryable<Comment> commentSource)
            {
                source = (IQueryable<T>) commentSource.OrderComments(feedQuery.Order);
            }

            return source
                .Skip(feedQuery.Cursor) 
                .Take(feedQuery.Limit); 
        }
    }
}