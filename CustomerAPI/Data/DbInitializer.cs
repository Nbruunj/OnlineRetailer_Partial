using System.Collections.Generic;
using System.Linq;
using System;
using CustomerAPI.Models;

namespace CustomerAPI.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(CustomerApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Products
            if (context.Customer.Any())
            {
                return;   // DB has been seeded
            }

            List<Customer> customers = new List<Customer>
            {
                new Customer
                {
                    Id = 1,
                    Name = "Poul",
                    Phone = 24728592,
                    Email = "Poul@Poul.dk",
                    BillingAddress = "Frodesgade 48, st. th",
                    ShippingAddress = "Skjoldsgade 18, 2. th",
                    CreditStanding = "Meh"
                }
            };

            context.Customer.AddRange(customers);
            context.SaveChanges();
        }
    }
}
