﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shardmodels
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int? customerId { get; set; }
        public OrderStatus Status { get; set; }
        public IList<OrderLine> OrderLines { get; set; }

        public enum OrderStatus
        {
            cancelled,
            completed,
            shipped,
            paid
        }
    }
    public class OrderLine
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
