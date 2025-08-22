//import .Net and SQLite namespaces
using System;
using SQLite;

//map the class to a table in the database named Customers
[Table("Customers")]
public class Customer
{
    [PrimaryKey, AutoIncrement]
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ContactPhone { get; set; }
    public string Email { get; set; }

    // Default constructor
    public Customer() { }
    // Parameterless constructor for SQLite

    // Constructor with parameters for instantiation purposes
    public Customer(string firstName, string lastName, string contactPhone, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        ContactPhone = contactPhone;
        Email = email;
    }
}
