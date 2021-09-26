using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace CustomerManagement.Models
{
    public partial class Card
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public CardType Type { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Cvv { get; set; }
        public long CustomerId { get; set; }

        //public virtual Customer Customer { get; set; }
    }
}
