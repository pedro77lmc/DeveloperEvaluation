using FluentValidation;
using static Application.DTOs.SaleRequest;

namespace Application.Validatiors
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        }
    }
}
