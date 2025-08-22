//Group 4: Ashley Shaw-Strand & Gregory Schultz
//ATTRIBUTIONS: librarymanagement.csv courtesy of Bimal Gajera via Kaggle Public Domain Dataset to initialize book catalogue

//import .NET and SQLite namespaces
using SQLite;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Components.Pages;

//using LibraryManagement namespace
namespace LibraryManagement.Resources.Data
{
    // This class represents a Book entity in the SQLite database
    public class BookServices
    {

        // The SQLiteAsyncConnection instance is used to interact with the SQLite database.
        private readonly SQLiteAsyncConnection _database;

        // Constructor sets the database path 
        public BookServices()
        {
            // Define the path to the SQLite database file.
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "librarymanagement.db");
            //open the database connection
            _database = new SQLiteAsyncConnection(dbPath);
            // Ensure the Book table is created in the database.
            _database.CreateTableAsync<Book>().Wait();
        }

        //Insert or replace a Book record in the database.
        public async Task AddBookAsync(Book book)
        {
            await _database.InsertAsync(book);
        }

        // Method to import books from a CSV file.
        public async Task ImportBooksFromCsvAsync(string filePath)
        {
            //clears all existing book records before importing new ones
            await _database.DeleteAllAsync<Book>();

            //open csv file and read it line by line
            using var reader = new StreamReader(filePath);
            string? line;
            bool isHeader = true;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                // Skip the header line which contains column names
                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                // Split the line by commas and parse the book data
                var parts = line.Split(',');
                if (parts.Length == 4)
                {
                    try
                    {
                        // Create a new Book object with the parsed data
                        var book = new Book
                        {
                            BookId = int.Parse(parts[0]),
                            Title = parts[1],
                            Author = parts[2],
                            Category = parts[3],
                        };
                        // Add the book to the database asynchronously
                        await AddBookAsync(book);
                    }
                    // Handle any parsing errors or exceptions
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing line: {line}\n{ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Malformed CSV line: {line}");
                }
            }
        }

        // Method to retrieve all book records from the database.
        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _database.Table<Book>().ToListAsync();
        }

        // Method to delete an existing book record in the database.
        public async Task DeleteBookAsync(int bookId)
        {
            // Remove book directly from the SQLite database
            await _database.DeleteAsync<Book>(bookId);
        }

        //searches database for book by its ID
        public async Task<Book?> GetBookByIdAsync(int bookId)
        {
            // Query the Book table to find matching book by its ID
            return await _database.Table<Book>()
                .FirstOrDefaultAsync(b => b.BookId == bookId);
        }
    }
}