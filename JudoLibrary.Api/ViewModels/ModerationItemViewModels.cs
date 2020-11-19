using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models.Moderation;

namespace JudoLibrary.Api.ViewModels
{
    public static class ModerationItemViewModels
    {
        // Assign to create delegate, projection expression that we compile, so we can use it in controller
        public static readonly Func<ModerationItem, object> Create = Projection.Compile();
        
        // Projection ViewModel
        // Expression holds information about function, returning a function
        // EF will take Expression and translate it into Select statement
        public static Expression<Func<ModerationItem, object>> Projection =>
            // Accept ModerationItem and return object, this function becomes object, we are mapping stuff
            // to ModerationItem(moderationItem) object
            moderationItem => new
            {
                moderationItem.Id, // Crucial to editing
                moderationItem.Current,
                moderationItem.Target,
                moderationItem.Reason,
                moderationItem.Type,
                
                Updated = moderationItem.Updated.ToLocalTime().ToString("HH:mm dd/MM/yyyy"),
            };

    }
}