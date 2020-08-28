﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JudoLibrary.Data;
using JudoLibrary.Models;
using JudoLibrary.Models.Moderation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
    [Route("/api/moderation-items")]
    public class ModerationItemController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public ModerationItemController(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        // GET -> /api/moderation-items
        [HttpGet]
        public IEnumerable<ModerationItem> GetModerationItems() => _ctx.ModerationItems.ToList();
        
        // GET -> /api/moderation-items/{id}
        [HttpGet("{id}")]
        public ModerationItem GetModerationItem(int id) => _ctx.ModerationItems.FirstOrDefault(mi => mi.Id.Equals(id));
        
        // GET -> /api/moderation-items/{id}/comments
        [HttpGet("{id}/comments")]
        public IEnumerable<Comment> GetCommentsForModerationItem(int id) =>
            _ctx.ModerationItems
                // TEntity => source =>> {ModerationItems}, Expression => navigationPropertyPath =>> {mi => mi.Comments}
                // Include comments for moderation modItem
                .Include(mi => mi.Comments)
                // Where moderation modItem id is equal to id that is passed
                .Where(mi => mi.Id.Equals(id))
                // Select comments for single moderation modItem
                .Select(mi => mi.Comments)
                // Get list of comments for single moderation modItem
                .FirstOrDefault();
        
        // POST -> /api/moderation-items/{id}/comments
        // Create comment for particular moderation modItem
        // Passing id from url, and from body -> comment
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> CreateCommentForModerationItem(int id, [FromBody] Comment comment)
        {
            // Get particular moderation modItem 
            var moderationItem = _ctx.ModerationItems.FirstOrDefault(mi => mi.Id.Equals(id));
            
            // First group starts with 2nd @ symbol, \B -> non word boundary => means as soon it tied to some word without space, it
            // will ignore it. Allowing lower & upper case characters, numbers, dash and underscore, + -> one or more from collection
            // that is specified in ([]), ?<tag> is used to name group => (), used as reference to a group when we want to find that
            // particular regex group
            var regex = new Regex(@"\B(?<tag>@[a-zA-Z0-9-_]+)");

            // Assign original comment content that's being processed via LINQ, to Html comment content
            comment.HtmlContent = regex
                // This is what we want to match -> plain comment
                .Matches(comment.Content)
                // We start with comment.Content as initial value, 2nd param is func that takes string, which is original comment content
                // and match parameter which gives us Match options
                .Aggregate(comment.Content, (content, match) =>
                {
                    // Extract from regex <tag> group and grab the value => string
                    var tag = match.Groups["tag"].Value;

                    // Replacing matched regex tag which is a string, with link, so we have navigation, taggable/linkable ?user?
                    // And return that as content, which becomes link
                    return content.Replace(tag, $"<a href=\"{tag}-user-link\">{tag}</a>");
                });
            
            // Add comment to particular moderation modItem that we found by id, to list of comments
            moderationItem.Comments.Add(comment);
            
            // Save changes to DB
            await _ctx.SaveChangesAsync();
            
            // Return Ok response with created comment
            return Ok(comment);
        }
    }
}