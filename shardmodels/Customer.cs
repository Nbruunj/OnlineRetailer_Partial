using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shardmodels
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
