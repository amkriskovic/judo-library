using FluentValidation;

namespace JudoLibrary.Api.Form.Validation
{
    public class SubmissionFormValidation : AbstractValidator<SubmissionForm>
    {
        public SubmissionFormValidation()
        {
            RuleFor(x => x.TechniqueId).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Video).NotEmpty();
        }
    }
}