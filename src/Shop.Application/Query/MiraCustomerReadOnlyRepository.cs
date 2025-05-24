using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shop.Core.SharedKernel;
using Shop.Domain;

namespace Shop.Application.Query;

internal class MiraCustomerReadOnlyRepository()
    : IMiraCustomerReadOnlyRepository
{
    private static readonly List<Domain.Entities.CustomerAggregate.Customer> _customers = new List<Domain.Entities.CustomerAggregate.Customer>();
    public Task<IEnumerable<Domain.Entities.CustomerAggregate.Customer>> GetAllAsync()
    {
        return Task.FromResult(_customers.AsEnumerable());
    }

    public Task<Domain.Entities.CustomerAggregate.Customer> GetByIdAsync(Guid customerId)
    {
        return Task.FromResult(_customers.FirstOrDefault(x => x.Id == customerId));
    }

    public void AddOrUpdateCustomerProjection(Domain.Entities.CustomerAggregate.Customer customer)
    {
        var existing = _customers.FirstOrDefault(x => x.Id == customer.Id);
        if (existing == null)
        {
            _customers.Add(customer);
            Console.WriteLine($"Added Customer: {customer.Id}, count {_customers.Count}");
        }
        else
        {
            existing.ChangeEmail(customer.Email, false);
        }
    }
}