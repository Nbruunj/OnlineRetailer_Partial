using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using CustomerAPI.Models;

namespace CustomerAPI.Data
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly CustomerApiContext db;

        public CustomerRepository(CustomerApiContext context)
        {
            db = context;
        }

        Customer IRepository<Customer>.Add(Customer entity)
        {
            var newCustomer = db.Customer.Add(entity).Entity;
            db.SaveChanges();
            return newCustomer;
        }

        void IRepository<Customer>.Edit(Customer entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        Customer IRepository<Customer>.Get(int id)
        {
            return db.Customer.FirstOrDefault(o => o.Id == id);
        }

        void IRepository<Customer>.Remove(int id)
        {
            var order = db.Customer.FirstOrDefault(p => p.Id == id);
            db.Customer.Remove(order);
            db.SaveChanges();
        }
    }
}
