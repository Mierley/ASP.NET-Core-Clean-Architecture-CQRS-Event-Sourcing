using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Query.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories.Abstractions;

public interface IMiraCustomerReadOnlyRepository
{
    Task<IEnumerable<CustomerQueryModel>> GetAllAsync();
    Task<CustomerQueryModel> GetByIdAsync(Guid id);
}