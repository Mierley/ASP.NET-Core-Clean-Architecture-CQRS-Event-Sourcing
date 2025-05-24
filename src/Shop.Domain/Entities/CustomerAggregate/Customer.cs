using System;
using System.Collections.Generic;
using System.Linq;
using Shop.Core.SharedKernel;
using Shop.Domain.Entities.CustomerAggregate.Events;
using Shop.Domain.ValueObjects;

namespace Shop.Domain.Entities.CustomerAggregate;

public class Customer : BaseEntity, IAggregateRoot
{
    private bool _isDeleted;

    /// <summary>
    /// Initializes a new instance of the Customer class.
    /// </summary>
    /// <param name="firstName">The first name of the customer.</param>
    /// <param name="lastName">The last name of the customer.</param>
    /// <param name="gender">The gender of the customer.</param>
    /// <param name="email">The email address of the customer.</param>
    /// <param name="dateOfBirth">The date of birth of the customer.</param>
    public Customer(string firstName, string lastName, EGender gender, Email email, DateTime dateOfBirth,
        bool withEvent = true)
    {
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        Email = email;
        DateOfBirth = dateOfBirth;
        if (withEvent)
            AddDomainEvent(new CustomerCreatedEvent(Id, Version + 1, firstName, lastName, gender, email.Address,
                dateOfBirth));
    }

    /// <summary>
    /// Default constructor for Entity Framework or other ORM frameworks.
    /// </summary>
    public Customer()
    {
    }

    // Properties
    /// <summary>
    /// Gets the first name of the customer.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// Gets the last name of the customer.
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    /// Gets the gender of the customer.
    /// </summary>
    public EGender Gender { get; private set; }

    /// <summary>
    /// Gets or sets the email address of the customer.
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Gets the date of birth of the customer.
    /// </summary>
    public DateTime DateOfBirth { get; private set; }

    /// <summary>
    /// Changes the email address of the customer.
    /// </summary>
    /// <param name="newEmail">The new email address.</param>
    public void ChangeEmail(Email newEmail, bool withEvent = true)
    {
        if (Email.Equals(newEmail))
            return;

        Email = newEmail;
        if (withEvent)
            AddDomainEvent(new CustomerUpdatedEvent(Id, Version + 1, FirstName, LastName, Gender, newEmail.Address,
                DateOfBirth));
    }

    /// <summary>
    /// Deletes the customer.
    /// </summary>
    public void Delete(bool withEvent = true)
    {
        if (_isDeleted) return;

        _isDeleted = true;

        if (withEvent)
            AddDomainEvent(new CustomerDeletedEvent(Id, Version + 1, FirstName, LastName, Gender, Email.Address,
                DateOfBirth));
    }

    public void Replay(IEnumerable<CustomerBaseEvent> events)
    {
        events = events.ToList().OrderBy(baseEvent => baseEvent.Version);
        foreach (var e in events) Apply(e); // (если Version нужен)
    }

    private void Apply(BaseEvent e)
    {
        switch (e)
        {
            case CustomerCreatedEvent ev:
                this.FirstName = ev.FirstName;
                this.LastName = ev.LastName;
                this.Gender = ev.Gender;
                this.Email = Email.Create(ev.Email);
                this.DateOfBirth = ev.DateOfBirth;
                this.Version = ev.Version;
                this.Id = ev.Id;
                break;

            case CustomerDeletedEvent ev:
                Delete(false);
                break;

            case CustomerUpdatedEvent ev:
                ChangeEmail(Email.Create(ev.Email), false);
                break;
        }
    }
}