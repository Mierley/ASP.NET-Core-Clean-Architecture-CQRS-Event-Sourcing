using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shop.Core.SharedKernel;
using Shop.Domain;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Query.Abstractions;
using Shop.Query.Data.Repositories.Abstractions;
using Shop.Query.QueriesModel;

namespace Shop.Query.Data.Repositories;

internal class MiraCustomerReadOnlyRepository(
    IMiraEventStore miraEventStore,
    IMiraSnapshotRepository miraSnapshotRepository)
    : IMiraCustomerReadOnlyRepository
{
    public Task<IEnumerable<CustomerQueryModel>> GetAllAsync()
    {
        //todo-z Надо ли как-то получать все записи?
        throw new NotImplementedException();
    }

    public async Task<CustomerQueryModel> GetByIdAsync(Guid customerId)
    {
        //берём последний снепшот
        var snapshot = miraSnapshotRepository.GetLastSnapshot(customerId);
        var lastVersion = snapshot?.Version ?? 0;

        //новые события после снепшота
        var events = (await miraEventStore.LoadEventsAsync(customerId, lastVersion)).ToList();

        var customer = snapshot ?? new Customer(); // пустой экземпляр

        if (events.Any())
            customer.Replay(events); // восстанавливаем состояние

        return new CustomerQueryModel(events[0].AggregateId, customer);
    }
}