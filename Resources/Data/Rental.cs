//import .NET and SQLite namespaces
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Use the VillageRentals namespace
namespace LibraryManagement.Resources.Data
{
    // Creates a table named "Rentals" in the database instead of the default name
    [Table("Rentals")]

    // This class represents a Rental entity in the SQLite database
    public class Rental
    {
        // Primary key for the Rental table, auto-incremented the rentalId property
        [PrimaryKey, AutoIncrement]
        public int rentalId { get; set; }

        public DateTime CreatedDate { get; set; }
        public int customerId { get; set; }
        public string customerLastName { get; set; }
        public DateTime currentDate { get; set; }

        // Default constructor for Rental class SQLite
        // This constructor is required for SQLite to create instances of the class
        public Rental() { }

    }
}
