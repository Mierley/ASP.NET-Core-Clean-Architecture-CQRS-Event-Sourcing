using System;
using Eventuous;

namespace Shop.Domain.Entities.CustomerAggregate.Events;

[EventType("CustomerCreated")]
public class CustomerCreatedEvent(
    Guid id,
    int version,
    string firstName,
    string lastName,
    EGender gender,
    string email,
    DateTime dateOfBirth) : CustomerBaseEvent(id, version, firstName, lastName, gender, email, dateOfBirth);