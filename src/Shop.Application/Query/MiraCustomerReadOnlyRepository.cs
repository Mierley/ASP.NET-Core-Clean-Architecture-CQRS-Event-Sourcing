using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shop.Core.SharedKernel;
using Shop.Domain;

namespace Shop.Application.Query;

internal class MiraCustomerReadOnlyRepository(
    IMiraEventStore miraEventStore)
    : IMiraCustomerReadOnlyRepository
{
    public Task<IEnumerable<Domain.Entities.CustomerAggregate.Customer>> GetAllAsync()
    {
        //todo-z Надо ли как-то получать все записи?
        throw new NotImplementedException();
    }

    public async Task<Domain.Entities.CustomerAggregate.Customer> GetByIdAsync(Guid customerId)
    {
        //все-все события
        var events = (await miraEventStore.LoadEventsAsync(customerId)).ToList();
        if (events.Count == 0) return null;

        var customer = new Domain.Entities.CustomerAggregate.Customer();                 // пустой экземпляр
        customer.Replay(events);              // восстанавливаем состояние

        return customer;
    }
}