using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shop.Core.SharedKernel;
using Shop.Domain;

namespace Shop.Application.Query;

internal class MiraCustomerReadOnlyRepository(
    IMiraEventStore miraEventStore,
    IMiraSnapshotRepository miraSnapshotRepository)
    : IMiraCustomerReadOnlyRepository
{
    public Task<IEnumerable<Domain.Entities.CustomerAggregate.Customer>> GetAllAsync()
    {
        //todo-z Надо ли как-то получать все записи?
        throw new NotImplementedException();
    }

    public async Task<Domain.Entities.CustomerAggregate.Customer> GetByIdAsync(Guid customerId)
    {
        //берём последний снепшот
        var snapshot = miraSnapshotRepository.GetLastSnapshot(customerId);
        var lastVersion = snapshot?.Version ?? -1;

        //новые события после снепшота
        var events = (await miraEventStore.LoadEventsAsync(customerId, lastVersion)).ToList();

        var customer = snapshot ?? new Domain.Entities.CustomerAggregate.Customer(); // пустой экземпляр

        if (events.Any())
            customer.Replay(events); // восстанавливаем состояние

        return customer;
    }
}