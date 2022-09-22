using System;
using System.ComponentModel;

namespace CustomerAPI.Models
{
    public class Customer
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        [DefaultValue(true)]
        public bool CreditStanding { get; set; }
    }
}
