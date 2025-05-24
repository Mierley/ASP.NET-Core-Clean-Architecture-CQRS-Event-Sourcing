using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using Eventuous.Subscriptions;
using Eventuous.Subscriptions.Context;
using Shop.Application.Query;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Domain.Factories;
using Shop.Domain.ValueObjects;

namespace Shop.Infrastructure.Data.EventStore;

public class MiraEventsHandler(IMiraCustomerReadOnlyRepository repository) : IEventHandler
{
    public async ValueTask<EventHandlingStatus> HandleEvent(IMessageConsumeContext context)
    {
        Customer customer = null;
        switch (context.Message)
        {
            // ① Создание клиента
            case CustomerCreatedEvent newCreatedEvent:
                customer = new Customer(newCreatedEvent.FirstName, newCreatedEvent.LastName, newCreatedEvent.Gender, Email.Create(newCreatedEvent.Email),
                    newCreatedEvent.DateOfBirth, false);
                customer.Id = newCreatedEvent.Id;
                break;

            // ②  Обновление e-mail
            case CustomerUpdatedEvent newUpdatedEvent:
                customer = await repository.GetByIdAsync(newUpdatedEvent.Id);
                customer.Replay([newUpdatedEvent]);
                break;

            default:
                // игнорируем незнакомые события
                return await new ValueTask<EventHandlingStatus>(EventHandlingStatus.Ignored);
        }

        repository.AddOrUpdateCustomerProjection(customer);
        // если всё обработалось без ошибок:
        return await new ValueTask<EventHandlingStatus>(EventHandlingStatus.Success);
    }

    public string DiagnosticName { get; }
}