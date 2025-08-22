//Provides access to SQLite database and .NET MAUI file system for managing customer data.
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

//uses the LibraryManagement namespace
namespace LibraryManagement.Resources.Data
{


    // This class provides data access methods for Customer table in SQL
    public class CustomerDataService
    {
        // The SQLiteAsyncConnection instance is used to interact with the SQLite database.
        private readonly SQLiteAsyncConnection _database;

        // Constructor  sets up database connection and creates the Customers table if it does not exist.
        public CustomerDataService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "librarymanagement.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Customer>().Wait();
        }

        // Method to import customers from a CSV file into the database.
        public async Task ImportCustomersFromCsvAsync(string filePath)
        {

            using var reader = new StreamReader(filePath);
            string? line;
            bool isHeader = true;
            int importedCount = 0;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                var parts = line.Split(',');

                if (parts.Length >= 4)
                {
                    var email = parts[3].Trim().ToLower();
                    var existing = await _database.Table<Customer>()
                        .FirstOrDefaultAsync(c => c.Email.ToLower() == email);

                    if (existing == null)
                    {
                        var customer = new Customer
                        {
                            FirstName = parts[0].Trim(),
                            LastName = parts[1].Trim(),
                            ContactPhone = parts[2].Trim(),
                            Email = email
                        };

                        await _database.InsertAsync(customer);
                        importedCount++;
                    }
                }
            }

            Console.WriteLine($"Imported {importedCount} customers from CSV.");
        }


        // Method to add a new customer to the database asynchronously.
        public async Task<int> AddCustomerAsync(Customer customer)
        {
            // Validate phone number format: 555-555-5555
            if (!System.Text.RegularExpressions.Regex.IsMatch(customer.ContactPhone, @"^\d{3}-\d{3}-\d{4}$"))
            {
                throw new ArgumentException("Contact phone must be in the format 555-555-5555.");
            }

            // Check if a customer with the same email already exists
            var existing = await _database.Table<Customer>()
                .FirstOrDefaultAsync(c => c.Email == customer.Email);

            // If a customer with the same email exists, do not insert a new one
            if (existing != null)
            {
                return 0;
            }

            // Inserts a new customer into the database.
            return await _database.InsertAsync(customer);
        }

        // Method to update an existing customer in the database asynchronously.
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            // Retrieves all customers from the database asynchronously.
            return await _database.Table<Customer>().ToListAsync();
        }

        // Method to get a customer by their ID asynchronously.
        public async Task DeleteCustomerAsync(int customerId)
        {
            await _database.DeleteAsync<Customer>(customerId);
        }
    }
}
