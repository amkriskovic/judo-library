using FluentValidation;

namespace JudoLibrary.Api.Form.Validation
{
    public class CreateTechniqueFormValidation : AbstractValidator<CreateTechniqueForm>
    {
        public CreateTechniqueFormValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.SubCategory).NotEmpty();
        }
    }
}