using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;
using Shop.Core.SharedKernel;
using Shop.Domain;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Query.Abstractions;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories;

internal class CustomerReadOnlyRepository(
    IReadDbContext readDbContext,
    IEventStore eventStore,
    ISnapshotRepository snapshotRepository)
    : BaseReadOnlyRepository<CustomerQueryModel, Guid>(readDbContext), ICustomerReadOnlyRepository
{
    public async Task<IEnumerable<CustomerQueryModel>> GetAllAsync()
    {
        var sort = Builders<CustomerQueryModel>.Sort
            .Ascending(customer => customer.FirstName)
            .Descending(customer => customer.DateOfBirth);

        var findOptions = new FindOptions<CustomerQueryModel> {Sort = sort};

        using var asyncCursor = await Collection.FindAsync(Builders<CustomerQueryModel>.Filter.Empty, findOptions);
        return await asyncCursor.ToListAsync();
    }

    public new async Task<CustomerQueryModel> GetByIdAsync(Guid customerId)
    {
        //берём последний снепшот
        var snapshot = snapshotRepository.GetLastSnapshot(customerId);
        var lastVersion = snapshot?.Version ?? 0;

        //новые события после снепшота
        var events = (await eventStore.LoadEventsAsync(customerId, lastVersion)).ToList();

        var customer = snapshot ?? new Customer(); // пустой экземпляр

        if (events.Any())
            customer.Replay(events); // восстанавливаем состояние

        return new CustomerQueryModel(events[0].AggregateId, customer);
    }
}