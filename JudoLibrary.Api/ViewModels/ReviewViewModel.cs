using System;
using System.Linq.Expressions;
using JudoLibrary.Models.Moderation;

namespace JudoLibrary.Api.ViewModels
{
    public static class ReviewViewModel
    {
        public static Func<Review, object> Create = Projection.Compile();
        
        public static Expression<Func<Review, object>> Projection =>
            review => new
            {
                review.Id,
                review.ModerationItemId,
                review.Comment,
                review.Status
            };
    }
}