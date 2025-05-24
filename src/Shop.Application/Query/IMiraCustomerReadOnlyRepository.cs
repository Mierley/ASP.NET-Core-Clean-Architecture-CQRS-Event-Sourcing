using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Application.Query;

public interface IMiraCustomerReadOnlyRepository
{
    Task<IEnumerable<Domain.Entities.CustomerAggregate.Customer>> GetAllAsync();
    Task<Domain.Entities.CustomerAggregate.Customer> GetByIdAsync(Guid id);
    void AddOrUpdateCustomerProjection(Domain.Entities.CustomerAggregate.Customer customer);
}