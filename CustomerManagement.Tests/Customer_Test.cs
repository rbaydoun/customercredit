using System;
using Xunit;
using CustomerManagement.Datastore.Entities;

namespace CustomerManagement.Tests
{
  public class Customer_Test
  {
    [Fact]
    public void Test()
    {
      var customer = new Customer
      {
        Address = "I am an address",
        DateOfBirth = DateTime.Parse("12-12-2000"),
        Name = "Gerald"
      };

      Assert.True(customer.Name == "Gerald");

    }

    [Fact]
    public void TestAddCard()
    {
      var customer = new Customer
      {
        Address = "I am an address",
        DateOfBirth = DateTime.Parse("12-12-2000"),
        Name = "Gerald",
        Cards = new System.Collections.Generic.List<CreditCard>()
      };

      var card = new CreditCard
      { 
        CardNumber = "1234-1234-1234-1234",
        CVV = 231,
        ExpiryDate = "Some date",
        CardType = CreditCardType.Visa
      };

      customer.Cards.Add(card);

      Assert.True(customer.Cards.Count == 1);
    }
  }
}
