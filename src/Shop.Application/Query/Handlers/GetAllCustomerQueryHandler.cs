using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Shop.Application.Query.Queries;

namespace Shop.Application.Query.Handlers;

public class GetAllCustomerQueryHandler(IMiraCustomerReadOnlyRepository repository)
    : IRequestHandler<GetAllCustomerQuery, Result<IEnumerable<Domain.Entities.CustomerAggregate.Customer>>>
{

    public async Task<Result<IEnumerable<Domain.Entities.CustomerAggregate.Customer>>> Handle(
          GetAllCustomerQuery request,
          CancellationToken cancellationToken)
    {
        // This method will either return the cached data associated with the CacheKey
        // or create it by calling the GetAllAsync method.
        return Result<IEnumerable<Domain.Entities.CustomerAggregate.Customer>>.Success(
            await repository.GetAllAsync());
    }
}