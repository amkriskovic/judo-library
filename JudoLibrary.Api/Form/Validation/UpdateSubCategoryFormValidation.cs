using FluentValidation;

namespace JudoLibrary.Api.Form.Validation
{
    public class UpdateSubCategoryFormValidation : AbstractValidator<UpdateSubCategoryForm>
    {
        public UpdateSubCategoryFormValidation()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}