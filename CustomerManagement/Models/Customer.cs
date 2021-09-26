using System;
using System.Collections.Generic;

#nullable disable

namespace CustomerManagement.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Cards = new HashSet<Card>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
    }
}
