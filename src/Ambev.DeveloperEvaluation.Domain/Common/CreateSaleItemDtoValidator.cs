using FluentValidation;
using static Application.DTOs.SaleRequest;

namespace Application.Validatiors
{
    public class CreateSaleItemDtoValidator : AbstractValidator<CreateSaleItemDto>
    {
        public CreateSaleItemDtoValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(20);

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0);

            RuleFor(x => x.Product)
                .NotNull()
                .SetValidator(new ProductDtoValidator());
        }
    }
}
