using System;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Query.Abstractions;

namespace Shop.Query.QueriesModel;

public class CustomerQueryModel : IQueryModel<Guid>
{
    public CustomerQueryModel(
        Guid id,
        string firstName,
        string lastName,
        string gender,
        string email,
        DateTime dateOfBirth)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        Email = email;
        DateOfBirth = dateOfBirth;
    }

    public CustomerQueryModel(Guid customerId, Customer customer)
    {
        Id = customerId;
        FirstName = customer.FirstName;
        LastName = customer.LastName;
        Gender = customer.Gender.ToString();
        Email = customer.Email.ToString();
        DateOfBirth = customer.DateOfBirth;
    }

    private CustomerQueryModel()
    {
    }

    public Guid Id { get; private init; }
    public string FirstName { get; private init; }
    public string LastName { get; private init; }
    public string Gender { get; private init; }
    public string Email { get; private init; }
    public DateTime DateOfBirth { get; private init; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}