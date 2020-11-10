using System;
using System.Linq;
using System.Linq.Expressions;
using JudoLibrary.Models;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Api.ViewModels
{
    public static class SubmissionViewModels
    {
        public static readonly Func<Submission, object> Created = Projection.Compile();

        public static Expression<Func<Submission, object>> Projection =>
            submission => new
            {
                submission.Id,
                submission.Description,
                Thumb = submission.Video.ThumbLink,
                Video = submission.Video.VideoLink,
                Created = submission.Created
                    .ToLocalTime()
                    .ToString("HH:mm dd/MM/yyyy"),
                Score = submission.Votes.AsQueryable().Sum(sv => sv.Value),
                Vote = 0,
                User = new
                {
                    submission.User.Image,
                    submission.User.Username
                }
            };


        public static Expression<Func<Submission, object>> PerspectiveProjection(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Projection;
            }

            return submission => new
            {
                submission.Id,
                submission.Description,
                Thumb = submission.Video.ThumbLink,
                Video = submission.Video.VideoLink,
                Created = submission.Created
                    .ToLocalTime()
                    .ToString("HH:mm dd/MM/yyyy"),
                Score = submission.Votes.AsQueryable().Sum(sv => sv.Value),
                Vote = submission.Votes
                    .AsQueryable()
                    .Where(sv => sv.UserId == userId)
                    .Select(sv => sv.Value)
                    .FirstOrDefault(),
                User = new
                {
                    submission.User.Image,
                    submission.User.Username
                }
            };

        }
    }
}