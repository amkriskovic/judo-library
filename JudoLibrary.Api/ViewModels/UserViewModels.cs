using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Api.ViewModels
{
    public static class UserViewModel    
    {
        public static readonly Func<User, object> CreateFlatCache = FlatProjection.Compile();
        
        public static object CreateFlat(User user) => CreateFlatCache(user);

        public static Expression<Func<User, object>> FlatProjection =>
            user => new
            {
                user.Id,
                user.Username,
                user.Image
            };
    }
}