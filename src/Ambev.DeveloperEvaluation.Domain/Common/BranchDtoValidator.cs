using FluentValidation;
using static Application.DTOs.SaleRequest;

namespace Application.Validatiors
{
    public class BranchDtoValidator : AbstractValidator<BranchDto>
    {
        public BranchDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.State).NotEmpty().MaximumLength(2);
        }
    }
}
