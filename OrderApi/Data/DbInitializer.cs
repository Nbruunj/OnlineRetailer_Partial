using System.Collections.Generic;
using System.Linq;
using OrderApi.Models;
using System;

namespace OrderApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(OrderApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Products
            if (context.Orders.Any())
            {
                return;   // DB has been seeded
            }


            List<OrderLine> orderLines = new List<OrderLine>
            {
                new OrderLine { OrderId = 1, ProductId = 2, Quantity = 3}
            };
            context.OrderLines.AddRange(orderLines);
            context.SaveChanges();

            List<Order> orders = new List<Order>
            {
                new Order {Date = DateTime.Today, OrderLines = orderLines, Status = Order.OrderStatus.completed, customerId = 1}
            };

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}
