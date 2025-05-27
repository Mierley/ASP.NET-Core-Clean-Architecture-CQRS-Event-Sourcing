using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Shop.Application.Customer.Commands;
using Shop.Application.Customer.Responses;
using Shop.Core.SharedKernel;
using Shop.Domain.ValueObjects;

namespace Shop.Application.Customer.Handlers;

public class CreateCustomerCommandHandler(
    IValidator<CreateCustomerCommand> validator,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCustomerCommand, Result<CreatedCustomerResponse>>
{
    public async Task<Result<CreatedCustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        // Validating the request.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Return the result with validation errors.
            return Result<CreatedCustomerResponse>.Invalid(validationResult.AsErrors());
        }

        // Instantiating the Email value object.
        var email = Email.Create(request.Email).Value;

        // Creating an instance of the customer entity.
        // When instantiated, the "CustomerCreatedEvent" will be created.
        var customer = new Domain.Entities.CustomerAggregate.Customer(
            request.FirstName,
            request.LastName,
            request.Gender,
            email,
            request.DateOfBirth);

        // Saving changes to the database and triggering events.
        await unitOfWork.SaveChangesAsync(customer);

        // Returning the ID.
        return Result<CreatedCustomerResponse>.Created(
            new CreatedCustomerResponse(customer.Id), location: $"/api/customers/{customer.Id}");
    }
}