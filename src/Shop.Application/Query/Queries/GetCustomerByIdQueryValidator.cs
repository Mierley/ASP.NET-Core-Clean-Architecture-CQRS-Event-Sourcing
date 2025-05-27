using FluentValidation;

namespace Shop.Application.Query.Queries;

public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}