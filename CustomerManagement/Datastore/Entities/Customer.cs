using System;
using System.Collections.Generic;

namespace CustomerManagement.Datastore.Entities
{
  public class Customer
  {
    public string Name { get; set; }
    public string Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public List<CreditCard> Cards { get; set; }
  }
}
