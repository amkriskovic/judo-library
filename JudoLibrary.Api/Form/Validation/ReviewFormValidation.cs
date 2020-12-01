using FluentValidation;
using JudoLibrary.Models.Moderation;

namespace JudoLibrary.Api.Form.Validation
{
    public class ReviewFormValidation : AbstractValidator<ModerationItemReviewContext.ReviewForm>
    {
        public ReviewFormValidation()
        {
            RuleFor(x => x.Comment)
                .NotEmpty()
                .When(x => x.Status != ReviewStatus.Approved);
            
            // RuleFor(x => x.Status).NotEmpty();
        }
    }
}