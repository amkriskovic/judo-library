using System;
using System.Linq.Expressions;
using JudoLibrary.Models.Moderation;

namespace JudoLibrary.Api.ViewModels
{
    public static class ReviewViewModel
    {
        public static readonly Func<Review, object> Create = Projection.Compile();
        
        public static Expression<Func<Review, object>> Projection =>
            review => new
            {
                review.Id,
                review.ModerationItemId,
                review.Comment,
                review.Status,
            };
        
        
        public static Func<Review, object> CreateWithUser = WithUserProjection.Compile();
        
        public static Expression<Func<Review, object>> WithUserProjection =>
            review => new
            {
                review.Id,
                review.ModerationItemId,
                review.Comment,
                review.Status,
                User = UserViewModel.CreateFlat(review.User)
            };
    }
}