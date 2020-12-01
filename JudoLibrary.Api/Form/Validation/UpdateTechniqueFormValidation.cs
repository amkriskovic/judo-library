using FluentValidation;

namespace JudoLibrary.Api.Form.Validation
{
    public class UpdateTechniqueFormValidation : AbstractValidator<UpdateTechniqueForm>
    {
        public UpdateTechniqueFormValidation()
        {
            RuleFor(x => x.Id).NotEqual(0);
            RuleFor(x => x.Reason).NotEmpty(); 
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.SubCategory).NotEmpty();
        }
    }
}