using System;
using System.Collections.Generic;

#nullable disable

namespace CustomerManagement.Datastore.Models
{
    public partial class Card
    {
        public string Number { get; set; }
        public CardType Type { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Cvv { get; set; }
        public long? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
