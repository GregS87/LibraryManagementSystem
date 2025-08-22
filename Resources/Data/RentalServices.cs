//import SQLite and .NET namespaces
using LibraryManagement.Components.Layout;
using Microsoft.Maui.Storage;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Resources.Data;


//manages interactions with the SQLite database for rental operations
public class RentalServices
{
    //holds connection to the SQLite database
    private readonly SQLiteAsyncConnection _database;

    // Constructor initializes the database connection and creates necessary tables when creating new RentalServices instance.
    public RentalServices()
    {
        // Define the path to the SQLite database file.
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "librarymanagement.db");
        // Open the database connection
        _database = new SQLiteAsyncConnection(dbPath);
        // Ensure the Rental and RentalItem tables are created in the database.
        _database.CreateTableAsync<Rental>().Wait();
        // Create the RentalItem table if it does not exist
        _database.CreateTableAsync<RentalItem>().Wait();
    }

    //Add a new rental with associated items
    public async Task<int> AddRentalAsync(Rental rental, List<RentalItem> items)
    {
        // Validate rental and items are inputted correctly
        if (rental == null || items == null || items.Count == 0)
            throw new ArgumentException("Rental and items must be provided.");

        foreach (var item in items)
        {
            // Validate each rental item
            bool available = await IsBookAvailableAsync(item.BookId, item.RentalDate, item.ReturnDate);
            // Ensure the rental item has a valid rental date and return date
            if (!available)
                throw new InvalidOperationException($"Book {item.BookId} is not available for the selected dates.");
        }
        // insert the rental into the database
        await _database.InsertAsync(rental);

        // Assign rentalId to each item and insert them into the database
        foreach (var item in items)
        {
            item.RentalId = rental.rentalId;
            await _database.InsertAsync(item);
        }

        return rental.rentalId;
    }

    // Retrieve a rental by its ID
    public async Task<Rental> GetRentalByIdAsync(int rentalId)
    {
        // Fetch the rental from the database using the provided rentalId
        var rental = await _database.Table<Rental>().FirstOrDefaultAsync(r => r.rentalId == rentalId);

        // If rental is found, get the rental items
        if (rental != null)
        {
            var items = await GetRentalItemsAsync(rentalId);
            if (items != null && items.Count > 0)
            {
                
                await _database.UpdateAsync(rental);
            }
        }
        return rental;
    }

    // get all items associated with a rental
    public async Task<List<RentalItem>> GetRentalItemsAsync(int rentalId)
    {
        //filters rentalitem by id and returns a list of RentalItem objects
        return await _database.Table<RentalItem>()
            .Where(ri => ri.RentalId == rentalId)
            .ToListAsync();
    }

    //get all rentals made by a specific customer
    public async Task<List<Rental>> GetRentalsByCustomerAsync(int customerId)
    {
        //filters rentals by customerId and returns a list of Rental objects
        return await _database.Table<Rental>()
            .Where(r => r.customerId == customerId)
            .ToListAsync();
    }


    // Method to delete the database file, useful for testing or resetting the database
    public static void DeleteDatabaseFile()
    {
        //check if file exists and delete it and logs
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "villagerentals.db");
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
            Console.WriteLine($"Deleted: {dbPath}");
        }
        else
        {
            Console.WriteLine($"File not found: {dbPath}");
        }
    }

    //checks if book is available for rental during the specified date range
    public async Task<bool> IsEquipmentAvailableAsync(int bookId, DateTime rentalDate, DateTime returnDate)
    {
        var overlappingRentals = await _database.Table<RentalItem>()
            .Where(item => item.BookId == bookId &&
                ((rentalDate < item.ReturnDate) && (returnDate > item.RentalDate)))
            .ToListAsync();

        return overlappingRentals.Count == 0;
    }

    public async Task<bool> IsBookAvailableAsync(int bookId, DateTime requestedStart, DateTime requestedEnd)
    {
        var rentals = await GetRentalItemsByBookIdAsync(bookId);
        return !rentals.Any(r =>
            requestedStart < r.ReturnDate && requestedEnd > r.RentalDate
        );
    }

    // Retrieves all rental items for a specific bookId
    public async Task<List<RentalItem>> GetRentalItemsByBookIdAsync(int bookId)
    {
        return await _database.Table<RentalItem>()
            .Where(ri => ri.BookId == bookId)
            .ToListAsync();
    }
}


