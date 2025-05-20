using System;
using Ardalis.Result;
using MediatR;

namespace Shop.Application.Query.Queries;

public class GetCustomerByIdQuery(Guid id) : IRequest<Result<Domain.Entities.CustomerAggregate.Customer>>
{
    public Guid Id { get; } = id;
}