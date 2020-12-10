using FluentValidation;

namespace JudoLibrary.Api.Form.Validation
{
    public class CreateSubCategoryFormValidation : AbstractValidator<CreateSubCategoryForm>
    {
        public CreateSubCategoryFormValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}