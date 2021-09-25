using System;
namespace CustomerManagement.Datastore.Entities
{
  public class CreditCard
  {
    public CreditCardType CardType { get; set; }
    public string CardNumber { get; set; }
    public string ExpiryDate { get; set; }
    public int CVV { get; set; }
  }
}
