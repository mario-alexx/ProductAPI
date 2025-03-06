using FluentValidation;

namespace ProductAPI.Models.Validators
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("The name cannot exceed 100 characters.");

            RuleFor(p => p.Price)
                .GreaterThan(0M).WithMessage("The price must be greater than 0.")
                .LessThan(1000000M).WithMessage("The price cannot exceed 10,000.");
        }
    }
}
