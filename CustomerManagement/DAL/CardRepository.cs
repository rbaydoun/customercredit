using System;
using System.Collections.Generic;
using System.Linq;
using CustomerManagement.Encryption;
using CustomerManagement.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.DAL
{
  public class CardRepository
  {
    private readonly IDataProtector protector;
    private readonly CustomerManagementContext dbContext;
    
    public CardRepository(
      CustomerManagementContext dbContext,
      IDataProtectionProvider dataProtectionProvider)
    {
      this.dbContext = dbContext;
      this.protector = dataProtectionProvider
        .CreateProtector(DataProtectionPurposeStrings.CreditCardInformation);
    }

    public void Insert(Card model)
    {
      var protectedNumber = protector.Protect(model.Number);
      var protectedCvv = protector.Protect(model.Cvv);

      model.Number = protectedNumber;
      model.Cvv = protectedCvv;
      dbContext.Cards.Add(model);
    }

    /// <summary>
    /// Get credit card by its ID
    /// </summary>
    /// <param name="id">The ID of the card.</param>
    /// <returns>The card with unencrypted data.</returns>
    public Card GetCardById(long id)
    {
      var card = dbContext.Cards.Find(id);
      var unprotectedNumber = protector.Unprotect(card.Number);
      var unprotectedCvv = protector.Unprotect(card.Cvv);

      card.Number = unprotectedNumber;
      card.Cvv = unprotectedCvv;

      return card;
    }

    /// <summary>
    /// Retrieve a credit card entry by number, owned by a customer.
    /// </summary>
    /// <param name="id">The Id of the customer owning the card.</param>
    /// <param name="number">The number of the card to remove.</param>
    /// <returns></returns>
    public Card GetByCustomerIdAndNumber(long id, string number)
    {
      var card = dbContext.Cards
        .Where(c => c.CustomerId == id)
        .ToList()
        .FirstOrDefault(c => protector.Unprotect(c.Number) == number);

      if (card != null)
      {        
        var unprotectedNumber = protector.Unprotect(card.Number);
        var unprotectedCvv = protector.Unprotect(card.Cvv);

        card.Number = unprotectedNumber;
        card.Cvv = unprotectedCvv;
        return card;
      }
      return card;
    }

    /// <summary>
    /// Updates a card entry in the data store.
    /// </summary>
    /// <param name="card">The card entity to update.</param>
    public void Update(Card card)
    {
      card.Number = protector.Protect(card.Number);
      card.Cvv = protector.Protect(card.Cvv);
    }

    /// <summary>
    /// Delete a specific card owned by a customer.
    /// </summary>
    /// <param name="id">The customer owning the card.</param>
    /// <param name="number">The number of the credit card to delete.</param>
    public void DeleteByCustomerIdAndNumber(long id, string number)
    {
      var cardToDelete = GetByCustomerIdAndNumber(id, number);

      if (cardToDelete != null)
      {
        dbContext.Cards.Remove(cardToDelete);
      }
    }
  }
}
