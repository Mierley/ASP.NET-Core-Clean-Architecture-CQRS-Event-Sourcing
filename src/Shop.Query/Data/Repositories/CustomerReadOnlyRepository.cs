using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Query.Abstractions;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories;

internal class CustomerReadOnlyRepository(IReadDbContext readDbContext, IEventStore eventStore)
    : BaseReadOnlyRepository<CustomerQueryModel, Guid>(readDbContext), ICustomerReadOnlyRepository
{
    public async Task<IEnumerable<CustomerQueryModel>> GetAllAsync()
    {
        var sort = Builders<CustomerQueryModel>.Sort
            .Ascending(customer => customer.FirstName)
            .Descending(customer => customer.DateOfBirth);

        var findOptions = new FindOptions<CustomerQueryModel>
        {
            Sort = sort
        };

        using var asyncCursor = await Collection.FindAsync(Builders<CustomerQueryModel>.Filter.Empty, findOptions);
        return await asyncCursor.ToListAsync();
    }

    public new async Task<CustomerQueryModel> GetByIdAsync(Guid customerId)
    {
        var events = (await eventStore.LoadEventsAsync(customerId)).ToList();

        if (events.Count == 0) return null;

        var customer = new Customer();                 // пустой экземпляр
        customer.Replay(events);              // восстанавливаем состояние
        return new CustomerQueryModel(events[0].AggregateId, customer);
    }
}