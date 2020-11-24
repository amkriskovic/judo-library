using System;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Api.ViewModels
{
    public static class CommentViewModels
    {
        // Assign to create delegate, projection expression that we compile, so we can use it in controller
        public static readonly Func<Comment, object> Create = Projection.Compile();
        
        // Projection expression that takes comment as input and returns object with props that we want
        public static Expression<Func<Comment, object>> Projection =>
            comment => new
            {
                comment.Id,
                comment.ParentId, // ParentId in case it is reply, created automatically by EF
                comment.Content, // Content in case we are editing the comment content
                comment.HtmlContent,
                User = UserViewModel.CreateFlat(comment.User),
            };
    }
}