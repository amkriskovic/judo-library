using FluentValidation;

namespace JudoLibrary.Api.Form.Validation
{
    public class UpdateStagedTechniqueFormValidation : AbstractValidator<UpdateTechniqueForm>
    {
        public UpdateStagedTechniqueFormValidation()
        {
            RuleFor(x => x.Id).NotEqual(0);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.SubCategory).NotEmpty();
        }
    }
}