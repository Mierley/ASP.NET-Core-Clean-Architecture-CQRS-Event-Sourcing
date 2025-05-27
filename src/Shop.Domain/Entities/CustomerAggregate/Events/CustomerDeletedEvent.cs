using System;

namespace Shop.Domain.Entities.CustomerAggregate.Events;

public class CustomerDeletedEvent(
    Guid id,
    int version,
    string firstName,
    string lastName,
    EGender gender,
    string email,
    DateTime dateOfBirth) : CustomerBaseEvent(id, version, firstName, lastName, gender, email, dateOfBirth);