using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Shop.Application.Query.Queries;

namespace Shop.Application.Query.Handlers;

public class GetCustomerByIdQueryHandler(
    IValidator<GetCustomerByIdQuery> validator,
    IMiraCustomerReadOnlyRepository repository) : IRequestHandler<GetCustomerByIdQuery, Result<Domain.Entities.CustomerAggregate.Customer>>
{
    public async Task<Result<Domain.Entities.CustomerAggregate.Customer>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        // Validating the request.
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Returns the result with validation errors.
            return Result<Domain.Entities.CustomerAggregate.Customer>.Invalid(validationResult.AsErrors());
        }

        return await repository.GetByIdAsync(request.Id);


        /*// Creating a cache key using the query name and the customer ID.
        var cacheKey = $"{nameof(GetCustomerByIdQuery)}_{request.Id}";

        // Getting the customer from the cache service. If not found, fetches it from the repository.
        // The customer will be stored in the cache service for future queries.
        var customer = await cacheService.GetOrCreateAsync(cacheKey, () => repository.GetByIdAsync(request.Id));

        // If the customer is null, returns a result indicating that no customer was found.
        // Otherwise, returns a successful result with the customer.
        return customer == null
            ? Result<CustomerQueryModel>.NotFound($"No customer found by Id: {request.Id}")
            : Result<CustomerQueryModel>.Success(customer);*/
    }
}