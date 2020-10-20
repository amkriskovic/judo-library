using System;
using System.Linq.Expressions;
using JudoLibrary.Models;

namespace JudoLibrary.Api.ViewModels
{
    public static class SubmissionViewModels
    {
        public static readonly Func<Submission, object> Create = Projection.Compile();

        public static Expression<Func<Submission, object>> Projection =>
            submission => new
            {
                submission.Id,
                submission.Description,
                Thumb = submission.Video.ThumbLink,
                Video = submission.Video.VideoLink,
                User = new
                {
                    submission.User.Image,
                    submission.User.Username
                }
            };
    }
}