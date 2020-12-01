using FluentValidation;

namespace JudoLibrary.Api.Form.Validation
{
    public class SubCategoryFormValidation : AbstractValidator<SubCategoryForm>
    {
        public SubCategoryFormValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}