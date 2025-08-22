//import SQLite and .NET namespaces
using SQLite;
using System;

//using Village Rentals namespace
namespace LibraryManagement.Resources.Data
{
    // Creates a table named "RentalItems" in the database instead of the default name
    [Table("RentalItems")]

    // This class represents a RentalItem entity in the SQLite database
    public class RentalItem
    {
        // Primary key for the RentalItem table, auto-incremented
        [PrimaryKey, AutoIncrement]
        public int RentalItemId { get; set; }
        public int RentalId { get; set; }
        public int BookId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}