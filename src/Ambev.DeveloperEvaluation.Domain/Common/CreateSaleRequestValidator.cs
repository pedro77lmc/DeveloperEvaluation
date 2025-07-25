namespace Application.Validatiors
{
    using FluentValidation;
    using static Application.DTOs.SaleRequest;

    public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
    {
        public CreateSaleRequestValidator()
        {
            RuleFor(x => x.SaleNumber)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.SaleDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.Now);

            RuleFor(x => x.Customer)
                .NotNull()
                .SetValidator(new CustomerDtoValidator());

            RuleFor(x => x.Branch)
                .NotNull()
                .SetValidator(new BranchDtoValidator());

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Sale must have at least one item");

            RuleForEach(x => x.Items)
                .SetValidator(new CreateSaleItemDtoValidator());
        }
    }

}
