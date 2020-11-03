using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JudoLibrary.Api.Form;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace JudoLibrary.Api
{
    public class CommentCreationContext
    {
        private readonly AppDbContext _ctx;
        private static Regex _tagMatch = new Regex(@"\B(?<tag>@[a-zA-Z0-9-_]+)", RegexOptions.Compiled);
        private string _userId;

        public CommentCreationContext(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public CommentCreationContext Setup(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));
            
            _userId = userId;
            
            return this;
        }

        public async Task<Comment> CreateCommentAsync(CommentForm commentForm)
        {
            // Init comment
            var comment = new Comment();

            // Perform checks based on comment type
            if (commentForm.ParentType == ParentType.ModerationItem)
            {
                if (!_ctx.ModerationItems.Any(x => x.Id == commentForm.ParentId))
                    throw new ParentNotFoundException("Moderation Item not found");

                comment.ModerationItemId = commentForm.ParentId;
                
            } else if (commentForm.ParentType == ParentType.Submission)
            {
                if (!_ctx.Submissions.Any(x => x.Id == commentForm.ParentId))
                    throw new ParentNotFoundException("Submission not found");
                
                comment.SubmissionId = commentForm.ParentId;
                
            } else if (commentForm.ParentType == ParentType.Comment)
            {
                if (!_ctx.Comments.Any(x => x.Id == commentForm.ParentId))
                    throw new ParentNotFoundException("Comment not found");
                
                comment.ParentId = commentForm.ParentId;
            }

            comment.Content = comment.Content;
            comment.UserId = _userId;
            
            // Assign original commentForm content that's being processed via LINQ, to Html comment content
            comment.HtmlContent = _tagMatch
                .Matches(commentForm.Content)
                .Aggregate(commentForm.Content, (content, match) =>
                {
                    // Extract from regex <tag> group and grab the value => string
                    var tag = match.Groups["tag"].Value;

                    return content.Replace(tag, $"<a href=\"{tag}-user-link\">{tag}</a>");
                });

            _ctx.Add(comment);
            await _ctx.SaveChangesAsync();

            // Assign User to Comment
            comment.User = _ctx.Users.AsNoTracking().FirstOrDefault(u => u.Id == _userId);

            return comment;
        }
        
        // Type of comment, for what comment is meant for
        public enum ParentType
        {
            ModerationItem = 0,
            Submission = 1,
            Comment = 2,
        }
        
        public class ParentNotFoundException : Exception
        {
            public ParentNotFoundException(string message) : base(message)
            {
                
            }
        }
    }
}