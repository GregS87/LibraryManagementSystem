//import .NET 9.0 and SQLite namespaces
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// use the LibraryManagement namespace
namespace LibraryManagement.Resources.Data


{ // Creates a table named "Equipment" in the database instead of default name
    [Table("Book")]

    // This class represents a Book entity in the SQLite database
    public class Book
    {

        [PrimaryKey]
        public int BookId { get; set; }
        public string Category { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }

        // Default constructor for Book class
        public Book()
        {

        }

        // Parameterized constructor to initialize Books properties
        public Book(int bookId, string category, string title, string author)
        {
            this.BookId = bookId;
            this.Category = category;
            this.Title = title;
            this.Author = author;

        }
    }
}
