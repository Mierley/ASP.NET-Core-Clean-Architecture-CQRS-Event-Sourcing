using System.Collections.Generic;
using Ardalis.Result;
using MediatR;

namespace Shop.Application.Query.Queries;

public class GetAllCustomerQuery : IRequest<Result<IEnumerable<Domain.Entities.CustomerAggregate.Customer>>>;